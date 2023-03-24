module "splunk_vpc_subnet" {
  source       = "git::https://github.com/answerdigital/terraform-modules//Terraform_modules/vpc_subnets?ref=v1.0.0"
  owner        = var.splunk_project_owner
  project_name = var.splunk_project_name
  azs          = ["eu-west-2a"]
}

data "aws_ami" "amazon_linux_2" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-hvm-*-x86_64-ebs"]
  }
}

resource "aws_security_group" "ec2_sg" {
  #checkov:skip=CKV_AWS_260:Allowing ingress from 0.0.0.0 for public HTTP(S) access
  #checkov:skip=CKV2_AWS_5
  name        = "${var.splunk_project_name}-ec2-sg"
  description = "Security group for ec2_sg"
  vpc_id      = module.splunk_vpc_subnet.vpc_id

  ingress {
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    security_groups = [aws_security_group.lb_sg.id]
    description     = "Application Load Balancer"
  }

  egress {    
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
    description = "All traffic"
  }

  tags = {
    Name  = "${var.splunk_project_name}-ec2-sg"
    Owner = var.splunk_project_owner
  }
}

module "ec2_instance_setup" {
  source                 = "git::https://github.com/AnswerConsulting/AnswerKing-Infrastructure.git//Terraform_modules/ec2_instance?ref=v1.0.0"
  project_name           = var.splunk_project_name
  owner                  = var.splunk_project_owner
  ami_id                 = data.aws_ami.amazon_linux_2.id
  availability_zone      = "eu-west-2a"
  subnet_id              = module.splunk_vpc_subnet.public_subnet_ids[0]
  vpc_security_group_ids = [aws_security_group.ec2_sg.id]
  needs_elastic_ip       = false #true
  user_data = <<EOF
#!/bin/bash -xe
#logs all user_data commands into a user-data.log file
exec > >(tee /var/log/user-data.log|logger -t user-data -s 2>/dev/console) 2>&1

sudo yum update -y
sudo yum upgrade -y
sudo yum install docker -y
sudo systemctl enable docker.service
sudo systemctl start docker.service

sudo docker pull splunk/splunk:latest
sudo docker run -d -p 80:8000 -e "SPLUNK_START_ARGS=--accept-license" -e "SPLUNK_PASSWORD={password}" --name splunk splunk/splunk:latest
EOF
}

# route 53

resource "aws_route53_record" "splunk" {
  zone_id        = aws_route53_zone.hosted_zone.zone_id
  name           = var.splunk_domain_name
  type           = "A"
  ttl            = 300
  records        = [module.ec2_instance_setup.instance_public_ip_address] #[aws_lb.lb.dns_name]
}

resource "aws_route53_zone" "hosted_zone" {
  name = var.splunk_domain_name
}

# Elastic IP

resource "aws_eip" "lb_eip" {
  #checkov:skip=CKV2_AWS_19:IP is being used for load balancer
  vpc = true
  count = "2"

  tags = {
    Name  = "${var.splunk_project_name}-eip-${count.index}"
    Owner = var.splunk_project_owner
  }
}


# Load balancer

resource "aws_security_group" "lb_sg" {
  #checkov:skip=CKV_AWS_260:Allowing ingress from 0.0.0.0 for public HTTP(S) access
  #checkov:skip=CKV2_AWS_5
  name        = "${var.splunk_project_name}-lb-sg"
  description = "Security group for lb-sg"
  vpc_id      = module.splunk_vpc_subnet.vpc_id

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "HTTP"
  }

  ingress {    
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "HTTPS"
  }

  egress {    
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
    description = "All traffic"
  }

  tags = {
    Name  = "${var.splunk_project_name}-lb-sg"
    Owner = var.splunk_project_owner
  }
}

resource "aws_lb" "lb" {
  name                                    = "${var.splunk_project_name}-lb"
  internal                                = false
  load_balancer_type                      = "network"
  ip_address_type                         = "ipv4"

  dynamic "subnet_mapping" {
    for_each = module.splunk_vpc_subnet.public_subnet_ids
    content {
      subnet_id     = "${subnet_mapping.value}"
      allocation_id = "${aws_eip.lb_eip[subnet_mapping.key].id}"
    }
  }

  tags = {
    Name  = "${var.splunk_project_name}-lb"
  }
}

resource "aws_lb_target_group" "target_group" {
  name        = "${var.splunk_project_name}-lb-tg"
  port        = 443
  protocol    = "TCP"
  target_type = "alb"
  vpc_id      = module.splunk_vpc_subnet.vpc_id

  tags = {
    Name  = "${var.splunk_project_name}-lb-target-group"
  }

  lifecycle {
    create_before_destroy = true
    ignore_changes = [name]
  }
}

#resource "aws_acm_certificate" "cert" {
#  domain_name       = var.splunk_domain_name
#  validation_method = "DNS"
#
#  lifecycle {
#    create_before_destroy = true
#  }
#}

resource "aws_lb_listener" "lb_listener" {
  load_balancer_arn = aws_lb.lb.id
  port              = "80"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.id
  }
}

resource "aws_lb_listener" "lb_listener_443" {
  load_balancer_arn = aws_lb.lb.id
  port              = "443"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.id
  }
}

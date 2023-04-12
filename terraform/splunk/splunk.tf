module "splunk_vpc_subnet" {
  source       = "git::https://github.com/answerdigital/terraform-modules//Terraform_modules/vpc_subnets?ref=v1.0.0"
  owner        = var.splunk_project_owner
  project_name = var.splunk_project_name
  num_public_subnets = 2
  num_private_subnets = 0
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
    from_port       = 8000
    to_port         = 8089
    protocol        = "tcp"
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
  #source                 = "git::https://github.com/AnswerConsulting/AnswerKing-Infrastructure.git//Terraform_modules/ec2_instance?ref=v1.0.0"
  source                 = "./ec2"
  project_name           = var.splunk_project_name
  owner                  = var.splunk_project_owner
  ami_id                 = data.aws_ami.amazon_linux_2.id
  availability_zone      = module.splunk_vpc_subnet.az_zones[0]
  subnet_id              = module.splunk_vpc_subnet.public_subnet_ids[0]
  vpc_security_group_ids = [aws_security_group.ec2_sg.id]
  needs_elastic_ip       = false
  user_data_replace_on_change = true
  user_data = <<EOF
#!/bin/bash
set -ex
#logs all user_data commands into a user-data.log file
exec > >(tee /var/log/user-data.log|logger -t user-data -s 2>/dev/console) 2>&1
sudo yum update -y && yum upgrade -y
sudo amazon-linux-extras install docker -y
sudo service docker start
sudo docker pull splunk/splunk:latest
sudo docker run -d -p 8000:8000 -p 8089:8089 -e "SPLUNK_START_ARGS=--accept-license" -e "SPLUNK_PASSWORD=password" --name splunk splunk/splunk:latest
EOF
}

# Route53

data "aws_route53_zone" "hosted_zone" {
  name = var.dns_base_domain_name
}

resource "aws_acm_certificate" "cert" {
  domain_name       = var.dns_splunk_domain_name
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_route53_record" "splunk" {
  zone_id        = data.aws_route53_zone.hosted_zone.zone_id
  name           = var.dns_splunk_domain_name
  type           = "CNAME"
  set_identifier = "public_ip"
  ttl            = "60"
  records        = [aws_lb.lb.dns_name]

  geolocation_routing_policy {
    country = "GB"
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
  name                       = "${var.splunk_project_name}-lb"
  internal                   = false
  load_balancer_type         = "application"
  subnets                    = module.splunk_vpc_subnet.public_subnet_ids
  drop_invalid_header_fields = true
  security_groups            = [aws_security_group.lb_sg.id]
  enable_deletion_protection = true

  access_logs {
    bucket  = aws_s3_bucket.elb_logs.bucket
    enabled = true
  }

  tags = {
    Name  = "${var.splunk_project_name}-lb"
  }
}

resource "aws_lb_target_group" "target_group" {
  name        = "${var.splunk_project_name}-tg-${substr(uuid(), 0, 2)}"
  port        = 8000
  protocol    = "HTTP"
  target_type = "instance"
  vpc_id      = module.splunk_vpc_subnet.vpc_id

  health_check {
      path                = "/services/server/info"
      protocol            = "HTTP"
      port                = 8089
      matcher             = "200"
      interval            = 15
      timeout             = 3
      healthy_threshold   = 2
      unhealthy_threshold = 2
    }


  tags = {
    Name  = "${var.splunk_project_name}-lb-target-group"
  }

  lifecycle {
    create_before_destroy = true
    ignore_changes        = [name]
  }
}

resource "aws_lb_target_group_attachment" "target_group_attachment_ec2" {
  target_group_arn = aws_lb_target_group.target_group.arn
  target_id        = module.ec2_instance_setup.ec2_id
  port             = 8000
}

resource "aws_lb_listener" "lb_listener_http" {
  load_balancer_arn = aws_lb.lb.arn
  port              = "80"
  protocol          = "HTTP"

  default_action {
    type = "redirect"

    redirect {
      port        = "443"
      protocol    = "HTTPS"
      status_code = "HTTP_301"
    }
  }
}

resource "aws_lb_listener" "lb_listener_https" {
  load_balancer_arn = aws_lb.lb.arn
  port              = "443"
  protocol          = "HTTPS"
  certificate_arn   = aws_acm_certificate.cert.arn

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.id
  }

  tags = {
    Name  = "${var.splunk_project_name}-lb-listener"
    Owner = var.splunk_project_owner
  }
}

# S3 logs
resource "aws_s3_bucket" "elb_logs" {
  bucket = "${var.splunk_project_name}-lb-logs"

  tags = {
    Name  = "${var.splunk_project_name}-lb-logs"
    Owner = var.splunk_project_owner
  }
}

data "aws_region" "current" {}
data "aws_caller_identity" "current" {}
data "aws_elb_service_account" "main" {}
resource "aws_s3_bucket_policy" "lb-bucket-policy" {
  bucket = aws_s3_bucket.elb_logs.id

  policy = <<POLICY
{
    "Id": "Policy",
    "Version": "2012-10-17",
    "Statement": [{
            "Effect": "Allow",
            "Principal": {
                "AWS": [
                    "${data.aws_elb_service_account.main.arn}"
                ]
            },
            "Action": [
                "s3:PutObject"
            ],
            "Resource": "${aws_s3_bucket.elb_logs.arn}/AWSLogs/*"
        },
        {
            "Effect": "Allow",
            "Principal": {
                "Service": "delivery.logs.amazonaws.com"
            },
            "Action": [
                "s3:PutObject"
            ],
            "Resource": "${aws_s3_bucket.elb_logs.arn}/AWSLogs/*",
            "Condition": {
                "StringEquals": {
                    "s3:x-amz-acl": "bucket-owner-full-control"
                }
            }
        },
        {
            "Effect": "Allow",
            "Principal": {
                "Service": "delivery.logs.amazonaws.com"
            },
            "Action": [
                "s3:GetBucketAcl"
            ],
            "Resource": "${aws_s3_bucket.elb_logs.arn}"
        }
    ]
}
POLICY
}

resource "aws_s3_bucket_acl" "elb_logs_bucket_acl" {
  bucket = aws_s3_bucket.elb_logs.id
  acl    = "private"

}

resource "aws_s3_bucket_server_side_encryption_configuration" "example" {
  bucket = aws_s3_bucket.elb_logs.bucket

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
    bucket_key_enabled = true
  }
}

resource "aws_s3_bucket_public_access_block" "elb_logs_backend_bucket_public_access_block" {
  bucket                  = aws_s3_bucket.elb_logs.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}
# Just putting everything in on file for now, will move around afterwards

# variables
variable "splunk_project_name" {
    type = string
    description = "Splunk Project Name"
    default = "answerking-splunk-instance"
}

variable "splunk_project_owner" {
    type = string
    description = "Splunk Resource Owner"
    default = "answerking"
}

module "splunk_vpc_subnet" {
  source               = "github.com/answerdigital/terraform-modules/modules/aws/vpc?ref=v1.1.1"
  owner                = var.splunk_project_owner
  project_name         = var.splunk_project_name
  enable_vpc_flow_logs = true
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
  name        = "${var.splunk_project_name}-ec2_sg"
  description = "Security group for ec2_sg"
  vpc_id       = module.splunk_vpc_subnet.vpc_id

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 443
    to_port     = 443
    protocol   = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name  = "${var.splunk_project_name}-ec2-sg"
    Owner = var.splunk_project_owner
  }
}

module "ec2_instance_setup" {
  source                 = "github.com/answerdigital/terraform-modules//modules/aws/ec2?ref=v1.1.1"
  project_name           = "answerking-splunk-instance"
  owner                  = "answerking"
  ami_id                 = data.aws_ami.amazon_linux_2.id
  availability_zone      = "eu-west-1"
  subnet_id              = module.splunk_vpc_subnet.public_subnet_ids[0]
  vpc_security_group_ids = [aws_security_group.ec2_sg.id]
  needs_elastic_ip       = true
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
sudo docker run -d -p 80:8000 -e "SPLUNK_START_ARGS=--accept-license" -e "SPLUNK_PASSWORD={secret password here}" --name splunk splunk/splunk:latest
EOF
}
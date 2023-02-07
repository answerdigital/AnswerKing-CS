variable "aws_region" {
  type        = string
  description = "AWS Region"
  default     = "eu-west-2"
}

variable "project_name" {
  type        = string
  description = "Project Name"
  default     = "answerking-dotnet-api"
}

variable "owner" {
  type        = string
  description = "Resource Owner"
  default     = "answerking-dotnet-team"
}

variable "dns_hosted_zone_id" {
  type        = string
  description = "ID of DNS Hosted Zone"
  default     = "Z0072706JT6B6N2J7Z9H"
}

variable "dns_record_name" {
  type        = string
  description = "DNS Record Name"
  default     = "dotnet.answerking.co.uk"
}

variable "dns_base_domain_name" {
  type        = string
  description = "DNS Base Domain Name"
  default     = "answerking.co.uk"
}

variable "tls_certificate_arn" {
  type        = string
  description = "TLS Certificate ARN"
  default     = "arn:aws:acm:eu-west-2:409973623162:certificate/4a5e41f6-af3d-49f0-bd20-67789b812321"
}

variable "image_url" {
  type        = string
  description = "AnswerKing C# API image"
  default     = "ghcr.io/answerconsulting/answerking-cs:latest"
}

variable "ec2_type" {
  type        = string
  description = "EC2 Instance Type"
  default     = "t2.micro"
}

variable "vpc_cidr" {
  type        = string
  description = "VPC CIDR"
  default     = "10.0.0.0/16"
}

variable "num_public_subnets" {
  type        = number
  description = "Number of public subnets"
  default     = 2
}

variable "num_private_subnets" {
  type        = number
  description = "Number of private subnets"
  default     = 2
}

variable "aws_cloudwatch_retention_in_days" {
  type        = number
  description = "AWS CloudWatch Logs Retention in Days"
  default     = 1
}

variable "efs_port" {
  type        = number
  description = "EFS Access Port"
  default     = 2049
}
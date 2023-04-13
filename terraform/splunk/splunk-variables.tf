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

variable "dns_base_domain_name" {
  type        = string
  description = "DNS Base Domain Name"
  default     = "answerking.co.uk"
}

variable "dns_splunk_domain_name" {
  type = string
  description = "Splunk Domain Name"
  default = "splunk.answerking.co.uk"
}
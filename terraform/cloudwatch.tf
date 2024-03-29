resource "aws_cloudwatch_log_group" "log_group" {
  #checkov:skip=CKV_AWS_158:TODO: encrypt logs in future security ticket
  name              = "${var.project_name}-logs"
  retention_in_days = var.aws_cloudwatch_retention_in_days

  tags = {
    Name = "${var.project_name}-logs"
    Owner = var.owner
  }
}

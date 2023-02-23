resource "aws_wafv2_web_acl" "wafv2_alb_acl" {
    name        = "${var.project_name}-acl"
    description = "Limits the rate at which one IP address can query the API"
    scope       = "REGIONAL"

    default_action {
        allow {}
    }

    rule {
        name     = "${var.project_name}-rate-limit"
        priority = 1

        action {
            block {}
        }
        statement {
            rate_based_statement {
                limit              = 300
                aggregate_key_type = "IP"
            }

        }
        visibility_config {
            cloudwatch_metrics_enabled = true
            metric_name                = "${var.project_name}-acl-rule-vis"
            sampled_requests_enabled   = false
        }
    }

    visibility_config {
        cloudwatch_metrics_enabled = true
        metric_name                = "${var.project_name}-acl-vis"
        sampled_requests_enabled   = false
    }
}

resource "aws_wafv2_web_acl_association" "web_acl_association_my_lb" {
    resource_arn = aws_lb.alb.arn
    web_acl_arn  = aws_wafv2_web_acl.wafv2_alb_acl.arn
}
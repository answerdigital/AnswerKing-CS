resource "aws_lb" "alb" {
  #checkov:skip=CKV_AWS_150:Deletion protection is being left off for ease of running terraform destroy

  #checkov:skip=CKV_AWS_91:TODO: Add cloudwatch logging
  #checkov:skip=CKV2_AWS_20:TODO: Redirect HTTP to HTTPS at load balancer and remove HTTP handling afterwards in future security ticket
  name               = "${var.project_name}-alb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.ecs_sg.id]
  subnets            = module.vpc_subnet.public_subnet_ids.*

  tags = {
    Name = "${var.project_name}-alb"
  }
}

resource "aws_lb_target_group" "alb_target_group" {
  name        = "${var.project_name}-alb-tg-${substr(uuid(), 0, 3)}"
  port        = 80
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = module.vpc_subnet.vpc_id

  health_check {
    healthy_threshold   = "3"
    interval            = "300"
    protocol            = "HTTP"
    matcher             = "200"
    timeout             = "3"
    path                = "/health"
    unhealthy_threshold = "2"
  }

  tags = {
    Name = "${var.project_name}-alb-tg"
  }

  lifecycle {
    create_before_destroy = true
    ignore_changes = [name]
  }
}

resource "aws_lb_target_group_attachment" "tg_attachment" {
    target_group_arn = aws_lb_target_group.nlb_target_group_forward_to_alb.arn
    target_id        = aws_lb.alb.arn
    port             = 80
}

resource "aws_lb_listener" "alb_listener" {
  load_balancer_arn = aws_lb.alb.id
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

resource "aws_lb_listener" "alb_listener_https" {
  load_balancer_arn = aws_lb.alb.id
  port              = "443"
  protocol          = "HTTPS"
  certificate_arn   = var.tls_certificate_arn
  ssl_policy        = "ELBSecurityPolicy-2016-08"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.alb_target_group.id
  }
}
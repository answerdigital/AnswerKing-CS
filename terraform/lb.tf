# Elastic IP

resource "aws_eip" "lb_eip" {
  vpc = true

  tags = {
    Name  = "${var.project_name}-eip"
    Owner = var.owner
  }
}

# Load Balancer

resource "aws_lb" "load_balancer" {
  name               = "${var.project_name}-lb"
  internal           = false
  load_balancer_type = "network"
  ip_address_type    = "ipv4"

  subnet_mapping {
    subnet_id     = "${module.vpc_subnet.public_subnet_ids[0]}"
    allocation_id = "${aws_eip.lb_eip.id}"
  }

  tags = {
    Name = "${var.project_name}-lb"
  }
}

resource "aws_lb_target_group" "target_group" {
  name        = "${var.project_name}-lb-tg"
  port        = 8000
  protocol    = "TCP"
  target_type = "ip"
  vpc_id      = module.vpc_subnet.vpc_id

  health_check {
    healthy_threshold   = "3"
    interval            = "300"
    protocol            = "TCP"
    matcher             = "200"
    timeout             = "3"
    path                = "/health"
    unhealthy_threshold = "2"
  }

  tags = {
    Name = "${var.project_name}-lb-tg"
  }
}

resource "aws_lb_listener" "listener" {
  load_balancer_arn = aws_alb.load_balancer.id
  port              = "8000"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.id
  }
}
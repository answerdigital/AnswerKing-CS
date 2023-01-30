terraform {
  backend "s3" {
    bucket = "answerking-dotnet-terraform"
    key    = "answerking-dotnet-terraform.tfstate"
    region = "eu-west-2"
  }
}

resource "aws_s3_bucket" "terraform_backend_bucket" {
  bucket = "answerking-dotnet-terraform"

  lifecycle {
    prevent_destroy = true
  }

  tags = {
    Name = "answerking-dotnet-terraform"
  }
}

resource "aws_s3_bucket_acl" "terraform_backend_bucket_acl" {
  bucket = aws_s3_bucket.terraform_backend_bucket.id
  acl    = "private"
}

resource "aws_s3_bucket_public_access_block" "terraform_backend_bucket_public_access_block" {
  bucket = aws_s3_bucket.terraform_backend_bucket.id

  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_s3_bucket_versioning" "terraform_backend_bucket_versioning" {
  bucket = aws_s3_bucket.terraform_backend_bucket.id
  versioning_configuration {
    status = "Enabled"
  }
}

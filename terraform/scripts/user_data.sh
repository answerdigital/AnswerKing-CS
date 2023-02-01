#!/bin/bash
exec > >(tee /var/log/user-data.log|logger -t user-data -s 2>/dev/console) 2>&1
cat <<'EOF' >> /etc/ecs/ecs.config
ECS_CLUSTER=${ECS_CLUSTER}
EOF
# install the REX-Ray Docker volume plugin
#docker plugin install rexray/ebs REXRAY_PREEMPT=true EBS_REGION=${EBS_REGION} --grant-all-permissions
# restart the ECS agent. This ensures the plugin is active and recognized once the agent starts.
#sudo systemctl restart ecs

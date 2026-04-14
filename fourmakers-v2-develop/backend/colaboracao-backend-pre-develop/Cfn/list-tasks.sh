. setenv
export ECS_CLUSTER=$(aws ecs list-clusters|jq -r '.clusterArns[0]')
echo "--- ecs cluster: ${ECS_CLUSTER}"
ecs-cli ps --cluster ${ECS_CLUSTER}|sort -t "/" -k 3

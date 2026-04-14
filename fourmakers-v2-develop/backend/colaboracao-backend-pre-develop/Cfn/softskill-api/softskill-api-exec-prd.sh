#!/usr/bin/env sh
# ecs update-service --cluster ecs --service srv-softskill-api-prd --enable-execute-command --force-new-deployment
set -e

CLUSTER="${CLUSTER:-ecs}"
SERVICE="${SERVICE:-srv-softskill-api-prd}"
CMD="${*:-/bin/sh}"

TASK="$(aws ecs list-tasks --cluster "$CLUSTER" --service-name "$SERVICE" --desired-status RUNNING --query 'taskArns[0]' --output text)"
[ "$TASK" = "None" ] && { echo "Nenhuma task RUNNING para $SERVICE no cluster $CLUSTER." >&2; exit 1; }

CONTAINER_ARG=""
[ -n "${CONTAINER:-}" ] && CONTAINER_ARG="--container $CONTAINER"

aws ecs execute-command --cluster "$CLUSTER" --task "$TASK" $CONTAINER_ARG --interactive --command "$CMD"

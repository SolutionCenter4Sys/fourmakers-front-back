#!/usr/bin/env sh
# ecs update-service --cluster ecs --service srv-reembolso-api-dev --enable-execute-command --force-new-deployment
set -e

CLUSTER="${CLUSTER:-ecs}"
SERVICE="${SERVICE:-srv-reembolso-api-dev}"
CMD="${*:-/bin/sh}"

TASK="$(aws ecs list-tasks --cluster "$CLUSTER" --service-name "$SERVICE" --desired-status RUNNING --query 'taskArns[0]' --output text)"
[ "$TASK" = "None" ] && { echo "Nenhuma task RUNNING para $SERVICE no cluster $CLUSTER." >&2; exit 1; }

aws ecs execute-command --cluster "$CLUSTER" --task "$TASK" --interactive --command "$CMD"

#!/usr/bin/env bash

docker --context default build -t ghcr.io/mixyboos/mixyboos-api .
docker --context default push ghcr.io/mixyboos/mixyboos-api

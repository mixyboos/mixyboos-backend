#!/usr/bin/env bash

docker --context default build -t ghcr.io/mixyboos/mixyboos-backend -f Dockerfile . --push

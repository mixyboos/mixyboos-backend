#!/usr/bin/env bash

cd hosting
docker --context mixyboos compose pull && \
    docker --context mixyboos compose down && \
    docker --context mixyboos compose up -d && \
    docker --context mixyboos compose logs -f

cd ..

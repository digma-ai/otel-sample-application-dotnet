#!/bin/bash
cd ../../../
docker build -t digma/sample.money-transfer-tester:latest -f ClientTester/ClientTester/Dockerfile .
docker run --rm --name	sample.money-transfer-tester --net digma-network  --env MoneyTransferApiUrl=http://money-transfer.api:7151  digma/sample.money-transfer-tester:latest

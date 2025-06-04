#!/bin/bash

kubectl create serviceaccount admin-user -n kubernetes-dashboard

kubectl create clusterrolebinding admin-user \
  --clusterrole=cluster-admin \
  --serviceaccount=kubernetes-dashboard:admin-user

echo "Fetching token..."
kubectl -n kubernetes-dashboard create token admin-user

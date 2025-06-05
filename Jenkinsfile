// Sample Jenkinsfile config
pipeline {
    agent {
        // Use Jenkins agent that has Docker CLI installed; 
        // if running Jenkins in Docker, it mounts the host Docker socket.
        label 'docker'
    }
    environment {
        DOCKER_REGISTRY = 'docker.io/itzreqle'        // Replace with your registry/username
        K8S_NAMESPACE   = 'fmss-dev'                  // Kubernetes namespace for dev deploys
        KUBECONFIG_CRED = 'kubeconfig-cred-id'        // Jenkins credential ID for kubeconfig
        GIT_CREDENTIALS = 'github-ssh-key'            // Jenkins SSH key credential ID (if using SSH)
    }
    options {
        // Keep build logs for 30 days
        buildDiscarder(logRotator(daysToKeepStr: '30'))
        skipDefaultCheckout()
    }
    stages {
        stage('Checkout') {
            steps {
                git branch: 'main',
                    credentialsId: env.GIT_CREDENTIALS,
                    url: 'git@github.com:itzreqle/fmss.git'
            }
        }

        stage('Build Docker Images') {
            parallel {
                stage('Build Frontend') {
                    steps {
                        dir('services/frontend') {
                            sh """
                              docker build -t \$DOCKER_REGISTRY/fmss-frontend:latest .
                            """
                        }
                    }
                }
                stage('Build Backend') {
                    steps {
                        dir('services/backend') {
                            sh """
                              docker build -t \$DOCKER_REGISTRY/fmss-backend:latest .
                            """
                        }
                    }
                }
                stage('Build Auth Service') {
                    steps {
                        dir('services/auth-api') {
                            sh """
                              docker build -t \$DOCKER_REGISTRY/fmss-auth:latest .
                            """
                        }
                    }
                }
                stage('Build DataLens') {
                    steps {
                        dir('services/datalens') {
                            sh """
                              docker build -t \$DOCKER_REGISTRY/fmss-datalens:latest .
                            """
                        }
                    }
                }
            }
        }

        stage('Push Docker Images') {
            steps {
                script {
                    // Login to Registry if credentials are needed
                    // withDockerRegistry([credentialsId: 'dockerhub-cred', url: 'https://index.docker.io/v1/']) {
                    //     // Login happens automatically inside this block
                    // }
                    sh """
                      docker push \$DOCKER_REGISTRY/fmss-frontend:latest
                      docker push \$DOCKER_REGISTRY/fmss-backend:latest
                      docker push \$DOCKER_REGISTRY/fmss-auth:latest
                      docker push \$DOCKER_REGISTRY/fmss-datalens:latest
                    """
                }
            }
        }

        stage('Integration Tests (Optional)') {
            when {
                expression { return fileExists('compose/docker-compose.dev.yml') }
            }
            steps {
                // Start up all services (local dev stack) and run any test suites
                sh 'docker-compose -f compose/docker-compose.dev.yml up -d --build'
                // Insert your test commands here, e.g.:
                // sh 'npm test --prefix services/backend'
                // sh 'npm test --prefix services/frontend'
                sh 'docker-compose -f compose/docker-compose.dev.yml down'
            }
        }

        stage('Deploy to Kubernetes (Dev)') {
            steps {
                withCredentials([file(credentialsId: env.KUBECONFIG_CRED, variable: 'KUBECONFIG_FILE')]) {
                    // Make KUBECONFIG available
                    sh 'export KUBECONFIG=\$KUBECONFIG_FILE'

                    // Update image tags in kustomize overlays if necessary (we used :latest above)
                    // If you pinned image digests or tags, you could use `kustomize edit set image ...`

                    dir('k8s/overlays/dev') {
                        // Apply manifests via kustomize
                        sh 'kubectl apply -k . -n \$K8S_NAMESPACE'
                    }
                }
            }
        }
    }

    post {
        always {
            cleanWs()  // Clean workspace after build
        }
        success {
            echo "Pipeline succeeded!"
        }
        failure {
            mail to: 'team@example.com',
                 subject: "Build #${env.BUILD_NUMBER} Failed",
                 body: "Check Jenkins console output for details: ${env.BUILD_URL}"
        }
    }
}

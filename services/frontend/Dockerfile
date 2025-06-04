# Use official Node.js 22 image
FROM node:22

# Set working directory
WORKDIR /app

# Install dependencies first (for better caching)
COPY package.json package-lock.json ./
RUN npm install

# Copy the rest of the application
COPY . .

# Expose the port Next.js runs on
EXPOSE 3000

# Start the development server
CMD ["npm", "run", "dev"] 

# Build the application
# RUN npm run build

# Start the production server
# CMD ["npm", "start"] 
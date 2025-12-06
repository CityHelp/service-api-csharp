#!/bin/bash

# Script para probar el sistema de autenticación JWT

echo "🔐 Testing JWT Authentication System"
echo "===================================="
echo ""

# Configuración
API_URL="http://localhost:5000"
HEALTH_ENDPOINT="$API_URL/api/testauth/health"
PROTECTED_ENDPOINT="$API_URL/api/testauth/protected"

# Colores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "📍 API URL: $API_URL"
echo ""

# Test 1: Health Check (sin autenticación)
echo "Test 1: Health Check (sin autenticación)"
echo "----------------------------------------"
response=$(curl -s -w "\n%{http_code}" "$HEALTH_ENDPOINT")
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if [ "$http_code" == "200" ]; then
    echo -e "${GREEN}✅ PASSED${NC} - Health endpoint respondió correctamente"
    echo "Response: $body"
else
    echo -e "${RED}❌ FAILED${NC} - Expected 200, got $http_code"
fi
echo ""

# Test 2: Acceso sin token (debe fallar con 401)
echo "Test 2: Acceso a endpoint protegido SIN token"
echo "----------------------------------------------"
response=$(curl -s -w "\n%{http_code}" "$PROTECTED_ENDPOINT")
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if [ "$http_code" == "401" ]; then
    echo -e "${GREEN}✅ PASSED${NC} - Correctamente rechazado (401 Unauthorized)"
    echo "Response: $body"
else
    echo -e "${RED}❌ FAILED${NC} - Expected 401, got $http_code"
fi
echo ""

# Test 3: Acceso con token inválido (debe fallar con 401)
echo "Test 3: Acceso a endpoint protegido CON token inválido"
echo "-------------------------------------------------------"
INVALID_TOKEN="eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.invalid.token"
response=$(curl -s -w "\n%{http_code}" -H "Authorization: Bearer $INVALID_TOKEN" "$PROTECTED_ENDPOINT")
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if [ "$http_code" == "401" ]; then
    echo -e "${GREEN}✅ PASSED${NC} - Token inválido correctamente rechazado"
    echo "Response: $body"
else
    echo -e "${RED}❌ FAILED${NC} - Expected 401, got $http_code"
fi
echo ""

# Test 4: Acceso con token válido (requiere token real del servicio Java)
echo "Test 4: Acceso a endpoint protegido CON token válido"
echo "-----------------------------------------------------"
echo -e "${YELLOW}⚠️  Para este test necesitas un token JWT válido del servicio Java${NC}"
echo ""
echo "Uso:"
echo "  export VALID_JWT_TOKEN='tu-token-aqui'"
echo "  ./test-jwt-auth.sh"
echo ""

if [ -n "$VALID_JWT_TOKEN" ]; then
    response=$(curl -s -w "\n%{http_code}" -H "Authorization: Bearer $VALID_JWT_TOKEN" "$PROTECTED_ENDPOINT")
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')

    if [ "$http_code" == "200" ]; then
        echo -e "${GREEN}✅ PASSED${NC} - Token válido aceptado"
        echo "Response: $body"
    else
        echo -e "${RED}❌ FAILED${NC} - Expected 200, got $http_code"
        echo "Response: $body"
    fi
else
    echo -e "${YELLOW}⏭️  SKIPPED${NC} - No se proporcionó VALID_JWT_TOKEN"
fi
echo ""

echo "===================================="
echo "✨ Tests completados"

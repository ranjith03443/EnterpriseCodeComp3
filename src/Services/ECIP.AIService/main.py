import logging
from contextlib import asynccontextmanager
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

import dependencies
from routers import health_router, gateway_router, prompt_router, settings_router, planner_router

logging.basicConfig(
    level=logging.INFO,
    format="[%(asctime)s] [%(levelname)s] %(name)s - %(message)s",
)
logger = logging.getLogger("ecip.ai_service")


@asynccontextmanager
async def lifespan(app: FastAPI):
    logger.info("ECIP AI Service starting...")
    dependencies.initialise()
    logger.info(
        "Gateway ready. LLM provider=%s  Embedding provider=%s",
        dependencies.get_config().active_provider,
        dependencies.get_config().active_embedding_provider,
    )
    yield
    logger.info("ECIP AI Service shutting down.")


app = FastAPI(
    title="ECIP AI Service",
    description="AI Gateway, Prompt Registry, and LLM abstraction layer for the Enterprise Code Intelligence Platform",
    version="1.0.0",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(health_router.router)
app.include_router(gateway_router.router)
app.include_router(prompt_router.router)
app.include_router(settings_router.router)
app.include_router(planner_router.router)


@app.get("/", tags=["Root"])
async def root():
    return {
        "service": "ECIP AI Service",
        "version": "1.0.0",
        "docs": "/docs",
    }

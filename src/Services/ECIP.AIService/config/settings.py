import json
import yaml
from pathlib import Path
from typing import Any


_BASE_DIR = Path(__file__).parent.parent
_CONFIG_FILE = _BASE_DIR / "config.yaml"
_OVERRIDE_FILE = _BASE_DIR / "ai_settings_override.json"


def _load_yaml() -> dict:
    if _CONFIG_FILE.exists():
        with open(_CONFIG_FILE, "r") as f:
            return yaml.safe_load(f) or {}
    return {}


def _load_override() -> dict:
    if _OVERRIDE_FILE.exists():
        with open(_OVERRIDE_FILE, "r") as f:
            return json.load(f)
    return {}


def _deep_merge(base: dict, override: dict) -> dict:
    result = dict(base)
    for key, value in override.items():
        if key in result and isinstance(result[key], dict) and isinstance(value, dict):
            result[key] = _deep_merge(result[key], value)
        else:
            result[key] = value
    return result


class AppConfig:
    def __init__(self):
        base = _load_yaml()
        override = _load_override()
        merged = _deep_merge(base, override)
        self._data: dict = merged

    @property
    def active_provider(self) -> str:
        return self._data.get("ai", {}).get("active_provider", "mock").lower()

    @property
    def active_embedding_provider(self) -> str:
        return self._data.get("ai", {}).get("active_embedding_provider", "mock").lower()

    @property
    def mock_delay_ms(self) -> int:
        return int(self._data.get("ai", {}).get("mock_delay_ms", 500))

    def provider_config(self, name: str) -> dict:
        return self._data.get("providers", {}).get(name, {})

    def embedding_config(self, name: str) -> dict:
        return self._data.get("embeddings", {}).get(name, {})

    def to_dict(self) -> dict:
        return dict(self._data)

    @staticmethod
    def save_override(data: dict) -> None:
        with open(_OVERRIDE_FILE, "w") as f:
            json.dump(data, f, indent=2)

    @staticmethod
    def reload() -> "AppConfig":
        return AppConfig()

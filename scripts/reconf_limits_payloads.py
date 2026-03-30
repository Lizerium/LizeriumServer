import json
import requests

TRANSLATE_URL = "http://192.168.64.128:5001/translate"

def translateFile(inputFile, outputFile):
    with open(inputFile, "r", encoding="utf-8") as f:
        data = json.load(f)

    for entry in data:
        if "description" in entry and isinstance(entry["description"], str):
            en_text = entry["description"]

            # Переводим через API
            response = requests.post(
                TRANSLATE_URL,
                data={
                    "q": en_text,
                    "source": "en",
                    "target": "ru",
                    "format": "text"
                }
            )
            ru_text = response.json().get("translatedText", "")

            print(en_text + " -> " + ru_text)

            # Заменяем поле description на объект
            entry["description"] = {
                "ru": ru_text,
                "en": en_text
            }

    # Сохраняем результат
    with open(outputFile, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    print("Готово! Сохранено в " + outputFile)


# Пример вызова
translateFile("..\LizeriumServerSrc\LizeriumServer\wwwroot\payloads\limit-breaking.json", "limit-breaking.json")

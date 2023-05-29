# HUSenderStudy
Jen taková studie, jak by mohla architektura senderu vypadat. Není spustitelná, kvůli minimalizaci času chybějí základní (víceméně mechanické) konstrukce (startup kód, konfigurace, konstruktory, čtení a zápis do DB, komunikace s partition managerem...) Pouze ideový návrh. 

Požadavky:
- paralelní zpracování jednotlivých kroků procesu: čtení z DB, odesílání do kafky, přijetí response z kafky, update stavu v databázi
- efektivní práce se seznamem suspended OBE
- korektní podpora dynamického škálování založeného na partitioningu dat podle OBE

Zatím implementováno:
- jednotlivé kroky procesu realizované samostatnými tasky + dispečer (background worker) řídící jejich životní cyklus
- podpora partitioningu

Zatím neimplementováno:
- práce se suspended OBE (včetně restartu)
- healthchecks
- ošetření výjimek včetně korektního ukončení aplikace při unrecoverable výjimce

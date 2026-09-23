CREATE TABLE IF NOT EXISTS "RssChannels"
(
    "Id"          SERIAL PRIMARY KEY,
    "Name"        VARCHAR(255) NOT NULL,
    "Url"         TEXT NOT NULL,
    "Description" TEXT,
    "CreatedAt"   TIMESTAMPTZ NOT NULL DEFAULT now(),
    "UpdatedAt"   TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS "IX_RssChannels_Name" ON "RssChannels" ("Name");

CREATE TABLE IF NOT EXISTS "NewsItems"
(
    "Id"           SERIAL PRIMARY KEY,
    "RssChannelId" INT NOT NULL REFERENCES "RssChannels" ("Id") ON DELETE CASCADE,
    "Title"        TEXT NOT NULL,
    "Description"  TEXT,
    "Link"         TEXT,
    "PublishedAt"  TIMESTAMPTZ NOT NULL,
    "CreatedAt"    TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS "IX_NewsItems_RssChannelId" ON "NewsItems" ("RssChannelId");
CREATE INDEX IF NOT EXISTS "IX_NewsItems_PublishedAt" ON "NewsItems" ("PublishedAt" DESC);

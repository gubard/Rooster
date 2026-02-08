using System.Collections.Frozen;
using System.Collections.Generic;

namespace Rooster.Contract.Helpers;

public static class RoosterMigration
{
    public static readonly FrozenDictionary<int, string> Migrations;

    static RoosterMigration()
    {
        Migrations = new Dictionary<int, string>
        {
            {
                23,
                @"
CREATE TABLE IF NOT EXISTS Alarms (
    Id TEXT PRIMARY KEY NOT NULL,
    Name TEXT NOT NULL CHECK(length(Name) <= 255),
    DueDateTime TEXT NOT NULL
);
"
            },
        }.ToFrozenDictionary();
    }
}

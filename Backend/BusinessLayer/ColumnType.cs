using System;

namespace Backend.BusinessLayer
{
    // Y - an enum is a named list of constants. This replaces magic numbers like 0, 1, 2 when referring to columns
    public enum ColumnType
    {
        // Y - Backlog is 0, InProgress is 1, Done is 2 — these numbers match the columnOrdinal in the API
        Backlog = 0,
        InProgress = 1,
        Done = 2
    }
}

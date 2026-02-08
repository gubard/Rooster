using Rooster.Contract.Models;

namespace Rooster.Contract.Helpers;

public static class Mapper
{
    public static Alarm ToAlarm(this AlarmEntity item)
    {
        return new()
        {
            Id = item.Id,
            Name = item.Name,
            DueDateTime = item.DueDateTime,
        };
    }

    public static AlarmEntity ToAlarmEntity(this Alarm item)
    {
        return new()
        {
            Id = item.Id,
            Name = item.Name,
            DueDateTime = item.DueDateTime,
        };
    }

    public static EditAlarm ToEditAlarm(this EditAlarmEntity item)
    {
        return new()
        {
            Id = item.Id,
            DueDateTime = item.DueDateTime,
            IsEditDueDateTime = item.IsEditDueDateTime,
            IsEditName = item.IsEditName,
            Name = item.Name,
        };
    }

    public static EditAlarmEntity ToEditAlarmEntity(this EditAlarm item)
    {
        return new(item.Id)
        {
            DueDateTime = item.DueDateTime,
            IsEditDueDateTime = item.IsEditDueDateTime,
            IsEditName = item.IsEditName,
            Name = item.Name,
        };
    }
}

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
            IsCompleted = item.IsCompleted,
        };
    }

    public static AlarmEntity ToAlarmEntity(this Alarm item)
    {
        return new()
        {
            Id = item.Id,
            Name = item.Name,
            DueDateTime = item.DueDateTime,
            IsCompleted = item.IsCompleted,
        };
    }

    public static EditAlarm ToEditAlarm(this EditAlarmEntity item)
    {
        return new()
        {
            Ids = [item.Id],
            DueDateTime = item.DueDateTime,
            IsEditDueDateTime = item.IsEditDueDateTime,
            IsEditName = item.IsEditName,
            Name = item.Name,
            IsCompleted = item.IsCompleted,
        };
    }

    public static IEnumerable<EditAlarmEntity> ToEditAlarmEntities(this EditAlarm item)
    {
        foreach (var id in item.Ids)
        {
            yield return new(id)
            {
                DueDateTime = item.DueDateTime,
                IsEditDueDateTime = item.IsEditDueDateTime,
                IsEditName = item.IsEditName,
                Name = item.Name,
                IsCompleted = item.IsCompleted,
                IsEditIsCompleted = item.IsEditIsCompleted,
            };
        }
    }
}

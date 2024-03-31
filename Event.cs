using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YouthUnion;

public partial class Event
{
    public int EventId { get; set; }

    [DisplayName("Название мероприятия")]
    public string? EventName { get; set; }

    [DisplayName("Место проведения")]
    public string? EventLocation { get; set; }

    [DisplayName("Дата начала")]
    public DateOnly StartDate { get; set; }

    [DisplayName("Дата окончания")]
    public DateOnly EndDate { get; set; }

    [DisplayName("Финансирование")]
    public decimal Financing { get; set; }

    public int? ResponsibleParticipantId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    [DisplayName("Ответственный")]
    public virtual Participant? ResponsibleParticipant { get; set; }

    public override string ToString()
    {
        return EventName;
    }
}

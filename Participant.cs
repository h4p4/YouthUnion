using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YouthUnion;

public partial class Participant
{
    public int ParticipantId { get; set; }

    [DisplayName("Полное имя")]
    public string? FullName { get; set; }

    [DisplayName("Контактная информация")]
    public string? ContactInfo { get; set; }

    [DisplayName("Возраст")]
    public int Age { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public override string? ToString()
    {
        return FullName;
    }
}

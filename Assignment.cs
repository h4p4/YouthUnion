using System;
using System.Collections.Generic;

namespace YouthUnion;

using System.ComponentModel;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public int? EventId { get; set; }

    public int? ParticipantId { get; set; }

    [DisplayName("Мероприятие")]
    public virtual Event? Event { get; set; }

    [DisplayName("Волонтёр")]
    public virtual Participant? Participant { get; set; }
}

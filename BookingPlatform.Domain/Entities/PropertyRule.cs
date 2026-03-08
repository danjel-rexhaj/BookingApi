using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Domain.Entities;

public class PropertyRule
{
    public Guid Id { get; private set; }

    public Guid PropertyId { get; private set; }
    public Property Property { get; private set; } = null!;

    public string RuleText { get; private set; } = null!;

    private PropertyRule() { }

    public PropertyRule(Guid propertyId, string ruleText)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        RuleText = ruleText;
    }
}
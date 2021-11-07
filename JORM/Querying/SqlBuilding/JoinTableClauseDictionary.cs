using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JORM.Querying.SqlBuilding
{
    public class JoinTableClauseDictionary
    {
        private readonly List<JoinTableClauses> _jtcs;
        private readonly StringComparison _nameComparison;

        public JoinTableClauseDictionary(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            _jtcs = new List<JoinTableClauses>();
            _nameComparison = comparison;
        }

        internal JoinTableClauses Register(JoinType join, text name, text alias)
        {
            if (alias.Length == 0)
            {
                foreach (var jtc in _jtcs)
                {
                    if (jtc.Join == join &&
                        TextHelper.Equals(jtc.Table.Name, name, _nameComparison))
                    {
                        return jtc;
                    }
                }
            }
            else
            {
                foreach (var jtc in _jtcs)
                {
                    if (jtc.Join == join &&
                        TextHelper.Equals(jtc.Table.Name, name, _nameComparison) &&
                        TextHelper.Equals(jtc.Table.Alias, alias, _nameComparison))
                    {
                        return jtc;
                    }
                }
            }
            {
                var table = new Table(name, alias);
                var jtc = new JoinTableClauses(join, table);
                _jtcs.Add(jtc);
                return jtc;
            }
        }

        internal bool TryLookup(text nameOrAlias, [NotNullWhen(true)] out JoinTableClauses jtc)
        {
            for (var i = 0; i < _jtcs.Count; i++)
            {
                jtc = _jtcs[i];
                // Alias is always the best match
                if (!string.IsNullOrWhiteSpace(jtc.Table.Alias) &&
                    TextHelper.Equals(jtc.Table.Alias, nameOrAlias, _nameComparison))
                    return true;
                // Name match is risky if we have multiple joins from the same table, but we'll let the user assume the risk
                if (TextHelper.Equals(jtc.Table.Name, nameOrAlias, _nameComparison))
                    return true;
            }

            jtc = null;
            return false;
        }

        internal bool TryGetNewest([NotNullWhen(true)] out JoinTableClauses jtc)
        {
            if (_jtcs.Count > 0)
            {
                jtc = _jtcs[0];
                return true;
            }

            jtc = null;
            return false;
        }

        internal JoinTableClauses GetFrom()
        {
            if (_jtcs.Count == 0)
                throw new SqlBuildException("There is no From Join");
            return _jtcs[0];
        }

        internal JoinTableClauses GetNewest()
        {
            if (!TryGetNewest(out var jtc))
                throw new SqlBuildException("There is no Join to reference");
            return jtc;
        }
    }
}
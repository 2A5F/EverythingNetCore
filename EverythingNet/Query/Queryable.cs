﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EverythingNet.Interfaces;
using IQueryable = EverythingNet.Interfaces.IQueryable;

namespace EverythingNet.Query
{
  internal abstract class Queryable : IQueryable, IQueryGenerator
  {
    private readonly IEverythingInternal everything;
    private IQueryGenerator parent;

    protected Queryable(IEverythingInternal everything, IQueryGenerator parent)
    {
      this.everything = everything;
      this.parent = parent;
    }

    public override string ToString()
    {
      return string.Join("", this.GetQueryParts());
    }

    public IEnumerator<string> GetEnumerator()
    {
      var search = this.everything.SendSearch(string.Join("", this.GetQueryParts()));

      return search.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public IQuery And => new LogicalQuery(this.everything, this, " ");

    public IQuery Or => new LogicalQuery(this.everything, this, "|");

    public virtual IEnumerable<string> GetQueryParts()
    {
      return this.parent?.GetQueryParts() ?? Enumerable.Empty<string>();
    }

    internal void SetParent(IQueryGenerator onTheFlyparent)
    {
      this.parent = onTheFlyparent;
    }

    protected string QuoteIfNeeded(string text)
    {
      if (text == null)
      {
        return string.Empty;
      }

      if (text.Contains(" ") && text.First() != '\"' && text.Last() != '\"')
      {
        return $"\"{text}\"";
      }

      return text;
    }
  }
}
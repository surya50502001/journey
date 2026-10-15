import React, { useState, useEffect } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { GitCompare, Plus, X, ArrowRight, ShieldAlert, Sparkles, Layers } from 'lucide-react';
import { api } from '../services/api';
import { BuilderComparisonResult, BuilderProfileSummary } from '../types';

export const ComparisonPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const [availableBuilders, setAvailableBuilders] = useState<BuilderProfileSummary[]>([]);
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [comparison, setComparison] = useState<BuilderComparisonResult | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadInitial();
  }, []);

  const loadInitial = async () => {
    try {
      const res = await api.getBuilders({ pageSize: 20 });
      if (res.success && res.data?.items) {
        setAvailableBuilders(res.data.items);

        // Pre-select 2 builders from query param or top 2
        const addParam = searchParams.get('add');
        let initial = res.data.items.slice(0, 2).map((b) => b.id);
        if (addParam && res.data.items.some((b) => b.id === addParam)) {
          initial = [addParam, ...initial.filter((id) => id !== addParam)].slice(0, 2);
        }
        setSelectedIds(initial);
        await runComparison(initial);
      }
    } catch {
      // fallback
    }
  };

  const runComparison = async (ids: string[]) => {
    if (ids.length < 1) {
      setComparison(null);
      return;
    }
    setLoading(true);
    try {
      const res = await api.compareBuilders(ids);
      if (res.success && res.data) {
        setComparison(res.data);
      }
    } catch {
      // fallback
    } finally {
      setLoading(false);
    }
  };

  const toggleBuilder = async (id: string) => {
    let next: string[];
    if (selectedIds.includes(id)) {
      if (selectedIds.length <= 1) return; // Keep at least 1
      next = selectedIds.filter((item) => item !== id);
    } else {
      if (selectedIds.length >= 3) {
        next = [...selectedIds.slice(1), id]; // Max 3
      } else {
        next = [...selectedIds, id];
      }
    }
    setSelectedIds(next);
    await runComparison(next);
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-8">
      {/* Header */}
      <div className="border-b border-stone-200 pb-6 space-y-1">
        <span className="text-xs font-mono uppercase tracking-wider text-brand-700 font-semibold flex items-center gap-1.5">
          <GitCompare className="w-3.5 h-3.5" />
          Cross-Journey Intelligence
        </span>
        <h1 className="text-3xl font-bold text-stone-950">Builder Journey Comparator</h1>
        <p className="text-stone-600 text-sm max-w-2xl">
          Side-by-side trajectory alignment: compare starting points, launch cadence, content mixes, and milestone pacing.
        </p>
      </div>

      {/* Builder selector bar */}
      <div className="bg-white p-4 rounded-xl border border-stone-200 shadow-2xs space-y-2">
        <div className="text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold">
          Select Builders to Compare (Select up to 3):
        </div>
        <div className="flex flex-wrap gap-2">
          {availableBuilders.map((b) => {
            const isSelected = selectedIds.includes(b.id);
            return (
              <button
                key={b.id}
                onClick={() => toggleBuilder(b.id)}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium border transition-colors flex items-center gap-2 ${
                  isSelected
                    ? 'bg-stone-900 text-stone-50 border-stone-900 shadow-xs'
                    : 'bg-stone-50 border-stone-200 text-stone-700 hover:bg-stone-100'
                }`}
              >
                <span>{b.name}</span>
                <span className="text-[10px] opacity-70 font-mono">({b.categoryName})</span>
                {isSelected ? <X className="w-3 h-3 text-stone-400" /> : <Plus className="w-3 h-3 text-stone-400" />}
              </button>
            );
          })}
        </div>
      </div>

      {loading ? (
        <div className="py-20 text-center text-xs text-stone-500 font-mono">
          Aligning multi-builder timelines and calculating comparative metrics...
        </div>
      ) : !comparison || comparison.builders.length === 0 ? (
        <div className="py-16 text-center bg-white rounded-xl border border-stone-200 text-sm text-stone-500">
          Select builders above to generate a side-by-side comparative analysis.
        </div>
      ) : (
        <div className="space-y-8">
          {/* Comparative Synthesis Card */}
          <div className="bg-white rounded-2xl border border-stone-200 p-6 space-y-3">
            <h3 className="text-sm font-bold text-stone-900 flex items-center gap-2">
              <Sparkles className="w-4 h-4 text-brand-600" />
              Strategic Divergence Synthesis
            </h3>
            <p className="text-xs sm:text-sm text-stone-700 leading-relaxed">
              {comparison.comparativeSynthesis}
            </p>
          </div>

          {/* Builder Profiles Header Cards */}
          <div className={`grid grid-cols-1 md:grid-cols-${comparison.builders.length} gap-6`}>
            {comparison.builders.map((b) => (
              <div
                key={b.id}
                className="bg-white rounded-2xl border border-stone-200 p-6 shadow-xs space-y-4"
              >
                <div className="flex items-center gap-3">
                  <img
                    src={b.avatarUrl}
                    alt={b.name}
                    className="w-12 h-12 rounded-full border border-stone-200 object-cover bg-stone-100"
                  />
                  <div>
                    <h4 className="font-bold text-stone-950 text-base">{b.name}</h4>
                    <span className="text-xs text-stone-500 font-mono">{b.handle}</span>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-2 text-xs font-mono pt-2 border-t border-stone-100">
                  <div>
                    <span className="text-stone-400 block text-[10px]">Category</span>
                    <span className="font-semibold text-stone-800">{b.categoryName}</span>
                  </div>
                  <div>
                    <span className="text-stone-400 block text-[10px]">Followers</span>
                    <span className="font-semibold text-stone-800">{b.followersCount.toLocaleString()}</span>
                  </div>
                </div>

                <div className="text-xs text-stone-600 bg-stone-50 p-2.5 rounded-lg border border-stone-100">
                  <span className="text-[10px] text-stone-400 font-mono block">Primary Resonance Style</span>
                  <span className="font-medium text-stone-800">{b.primaryContentStyle}</span>
                </div>

                <Link
                  to={`/builder/${b.id}`}
                  className="block text-center py-1.5 text-xs font-medium text-brand-800 bg-brand-50 hover:bg-brand-100 rounded-lg border border-brand-200"
                >
                  View Full Report
                </Link>
              </div>
            ))}
          </div>

          {/* Comparative Metrics Table */}
          <div className="bg-white rounded-2xl border border-stone-200 overflow-hidden shadow-xs">
            <div className="p-4 border-b border-stone-200 bg-stone-50/50">
              <h3 className="text-sm font-bold text-stone-900">Side-by-Side Metric Matrix</h3>
            </div>

            <div className="divide-y divide-stone-100">
              {comparison.comparativeMetrics.map((metric, i) => (
                <div key={i} className="p-4 flex flex-col sm:flex-row sm:items-center justify-between gap-3 text-xs">
                  <div className="sm:w-1/3 space-y-0.5">
                    <span className="font-semibold text-stone-900 block">{metric.metricName}</span>
                    <span className="text-[11px] text-stone-400 font-mono">{metric.contextNote}</span>
                  </div>

                  <div className={`sm:w-2/3 grid grid-cols-${comparison.builders.length} gap-4`}>
                    {comparison.builders.map((b) => (
                      <div key={b.id} className="font-mono text-stone-800 bg-stone-50 p-2 rounded-lg border border-stone-100">
                        <span className="text-[10px] text-stone-400 block sm:hidden font-sans font-semibold mb-1">
                          {b.name}
                        </span>
                        {metric.valuesByBuilderId[b.id] || '—'}
                      </div>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Non-causal analytical limitations notice */}
          <div className="bg-amber-50/70 border border-amber-200 rounded-xl p-4 text-xs text-amber-950 flex items-start gap-2.5">
            <ShieldAlert className="w-4 h-4 text-amber-700 shrink-0 mt-0.5" />
            <p className="leading-relaxed text-[11px]">
              {comparison.analyticalLimitationsNotice}
            </p>
          </div>
        </div>
      )}
    </div>
  );
};

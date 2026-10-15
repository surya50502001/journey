import React from 'react';
import { Layers, Zap, Hash, AlertTriangle } from 'lucide-react';
import { ContentStrategySummary } from '../types';

interface ContentDnaCardProps {
  strategy: ContentStrategySummary;
}

export const ContentDnaCard: React.FC<ContentDnaCardProps> = ({ strategy }) => {
  return (
    <div className="bg-white rounded-xl border border-stone-200 p-6 space-y-6">
      <div>
        <div className="flex items-center gap-2 text-stone-900 font-semibold text-base mb-1">
          <Layers className="w-5 h-5 text-brand-600" />
          <h3>Content Strategy & DNA Synthesis</h3>
        </div>
        <p className="text-stone-600 text-xs leading-relaxed">
          {strategy.strategyOverview}
        </p>
      </div>

      {/* Format Multipliers */}
      <div className="space-y-3">
        <h4 className="text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold flex items-center gap-1.5">
          <Zap className="w-3.5 h-3.5 text-amber-600" />
          Format Resonance Multipliers (Relative to Baseline)
        </h4>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {strategy.formatPerformances.map((f, i) => (
            <div
              key={i}
              className="p-3 rounded-lg border border-stone-200 bg-stone-50/70 flex items-center justify-between"
            >
              <div>
                <div className="text-xs font-medium text-stone-900">{f.formatName}</div>
                <div className="text-[11px] text-stone-500 font-mono mt-0.5">
                  {f.count} posts • avg {f.averageLikes} likes
                </div>
              </div>
              <div className="text-right">
                <span className="font-mono text-sm font-bold text-brand-700 bg-brand-50 px-2 py-0.5 rounded border border-brand-200">
                  {f.engagementMultiplier}x
                </span>
                <span className="block text-[10px] text-stone-400 mt-0.5">vs account median</span>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Recurring Discussion Topics */}
      <div className="space-y-3">
        <h4 className="text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold flex items-center gap-1.5">
          <Hash className="w-3.5 h-3.5 text-stone-500" />
          Recurring Discussion Clusters
        </h4>

        <div className="flex flex-wrap gap-2">
          {strategy.topTopics.map((topic, i) => (
            <span
              key={i}
              className="inline-flex items-center gap-1.5 px-3 py-1 rounded-lg bg-stone-100 text-stone-800 text-xs border border-stone-200"
            >
              <span className="font-medium">{topic.topic}</span>
              <span className="text-[10px] text-stone-500 font-mono">({topic.frequency}x)</span>
            </span>
          ))}
        </div>
      </div>

      {/* Causal Warning Disclaimer */}
      <div className="bg-stone-50 border border-stone-200 rounded-lg p-3 text-xs text-stone-500 flex items-start gap-2">
        <AlertTriangle className="w-4 h-4 text-stone-400 shrink-0 mt-0.5" />
        <p className="leading-relaxed text-[11px]">
          {strategy.limitationsNotice}
        </p>
      </div>
    </div>
  );
};

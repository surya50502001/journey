import React from 'react';
import { TrendingUp, ArrowRight, ExternalLink, ShieldAlert } from 'lucide-react';
import { Breakthrough } from '../types';

interface BreakthroughCardProps {
  breakthrough: Breakthrough;
}

export const BreakthroughCard: React.FC<BreakthroughCardProps> = ({ breakthrough }) => {
  return (
    <div className="bg-white rounded-xl border border-stone-200 p-6 space-y-4 hover:border-stone-300 transition-all">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
        <div className="flex items-center gap-2">
          <div className="w-7 h-7 rounded-lg bg-brand-100 text-brand-800 flex items-center justify-center">
            <TrendingUp className="w-4 h-4" />
          </div>
          <h4 className="text-base font-semibold text-stone-900">{breakthrough.title}</h4>
        </div>
        <span className="font-mono text-xs px-2.5 py-1 rounded-full bg-brand-50 text-brand-800 border border-brand-200 font-semibold self-start sm:self-auto">
          {breakthrough.magnitudeChange}
        </span>
      </div>

      <p className="text-stone-700 text-sm leading-relaxed">
        {breakthrough.summary}
      </p>

      {/* Metric Observed Tag */}
      <div className="bg-stone-50 border border-stone-200 rounded-lg p-2.5 text-xs text-stone-700 font-mono">
        <span className="text-stone-400">Observed Shift:</span> {breakthrough.metricObserved}
      </div>

      {/* Evidences timeline / before-and-after */}
      {breakthrough.evidences?.length > 0 && (
        <div className="space-y-2 pt-2">
          <span className="text-xs font-mono uppercase tracking-wider text-stone-400 block font-semibold">
            Correlated Posts & Evidence
          </span>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {breakthrough.evidences.map((ev, i) => (
              <div
                key={i}
                className="p-3 rounded-lg border border-stone-200 bg-white text-xs space-y-2 shadow-xs"
              >
                <div className="flex items-center justify-between text-[11px] text-stone-500">
                  <span className="font-medium text-brand-800 bg-brand-50/70 px-1.5 py-0.5 rounded">
                    {ev.roleInShift}
                  </span>
                  <span className="font-mono">{new Date(ev.publishedAt).toLocaleDateString()}</span>
                </div>
                <p className="text-stone-800 line-clamp-3 italic">"{ev.content}"</p>
                <div className="flex items-center justify-between text-[11px] text-stone-500 pt-1 border-t border-stone-100">
                  <span>{ev.likes} likes • {ev.replies} replies</span>
                  {ev.postUrl && (
                    <a
                      href={ev.postUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-stone-400 hover:text-stone-700 flex items-center gap-1"
                    >
                      Post <ExternalLink className="w-3 h-3" />
                    </a>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Non-causal Plausible Explanations */}
      <div className="text-xs text-stone-600 space-y-1 pt-1">
        <span className="font-semibold text-stone-800">Associated Pattern:</span>
        <p className="leading-relaxed">{breakthrough.plausibleExplanations}</p>
      </div>

      {/* Limitations Box */}
      <div className="bg-amber-50/50 border border-amber-200 rounded-lg p-3 text-xs text-amber-900 flex items-start gap-2">
        <ShieldAlert className="w-4 h-4 text-amber-700 shrink-0 mt-0.5" />
        <p className="leading-relaxed text-[11px]">
          {breakthrough.limitationsAndAlternatives}
        </p>
      </div>
    </div>
  );
};

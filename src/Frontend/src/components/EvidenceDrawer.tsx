import React from 'react';
import { X, ExternalLink, Heart, MessageCircle, Repeat2, Eye, Bookmark, ShieldAlert, CheckCircle2 } from 'lucide-react';
import { JourneyEventEvidence } from '../types';

interface EvidenceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  evidences: JourneyEventEvidence[];
  limitationsNote?: string;
  isInferred?: boolean;
}

export const EvidenceDrawer: React.FC<EvidenceDrawerProps> = ({
  isOpen,
  onClose,
  title,
  evidences,
  limitationsNote,
  isInferred
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto bg-stone-900/50 backdrop-blur-sm flex justify-end">
      <div className="bg-stone-50 w-full max-w-xl min-h-screen p-6 shadow-2xl border-l border-stone-200 flex flex-col justify-between animate-in slide-in-from-right duration-200">
        <div className="space-y-6">
          <div className="flex items-start justify-between border-b border-stone-200 pb-4">
            <div>
              <div className="flex items-center gap-2 mb-1">
                {isInferred ? (
                  <span className="inline-flex items-center gap-1 text-[11px] font-mono font-medium px-2 py-0.5 rounded bg-amber-100 text-amber-900 border border-amber-300">
                    <ShieldAlert className="w-3 h-3 text-amber-700" />
                    Inferred From Context
                  </span>
                ) : (
                  <span className="inline-flex items-center gap-1 text-[11px] font-mono font-medium px-2 py-0.5 rounded bg-emerald-100 text-emerald-900 border border-emerald-300">
                    <CheckCircle2 className="w-3 h-3 text-emerald-700" />
                    Explicit Builder Statement
                  </span>
                )}
              </div>
              <h3 className="text-lg font-semibold text-stone-950">{title}</h3>
            </div>
            <button
              onClick={onClose}
              className="p-1 rounded-md text-stone-400 hover:text-stone-700 hover:bg-stone-100"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Limitations Disclosure */}
          {limitationsNote && (
            <div className="bg-amber-50/70 border border-amber-200 rounded-lg p-3 text-xs text-amber-950 space-y-1">
              <span className="font-semibold block">Evidence Limitation Note:</span>
              <p className="text-amber-900/90 leading-relaxed">{limitationsNote}</p>
            </div>
          )}

          {/* Evidences list */}
          <div className="space-y-4">
            <h4 className="text-xs font-mono uppercase tracking-wider text-stone-500 font-semibold">
              Primary Source Posts ({evidences.length})
            </h4>

            {evidences.map((evidence, idx) => (
              <div
                key={evidence.id || idx}
                className="bg-white rounded-xl border border-stone-200 p-4 shadow-sm hover:border-stone-300 transition-all space-y-3"
              >
                <div className="flex items-center justify-between text-xs text-stone-500">
                  <span className="font-mono text-[11px]">
                    {new Date(evidence.publishedAt).toLocaleDateString(undefined, {
                      month: 'short',
                      day: 'numeric',
                      year: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit',
                    })}
                  </span>
                  <span className="bg-stone-100 px-2 py-0.5 rounded font-mono text-[10px] text-stone-600">
                    {evidence.evidenceType} (weight: {evidence.weight.toFixed(2)})
                  </span>
                </div>

                <p className="text-stone-900 text-sm whitespace-pre-line leading-relaxed font-normal">
                  "{evidence.postContent || evidence.excerpt}"
                </p>

                {/* Social Metrics */}
                <div className="flex items-center justify-between pt-2 border-t border-stone-100 text-xs text-stone-500">
                  <div className="flex items-center gap-4">
                    <span className="flex items-center gap-1 hover:text-rose-600">
                      <Heart className="w-3.5 h-3.5" />
                      {evidence.likes?.toLocaleString() || 0}
                    </span>
                    <span className="flex items-center gap-1 hover:text-sky-600">
                      <MessageCircle className="w-3.5 h-3.5" />
                      {evidence.replies?.toLocaleString() || 0}
                    </span>
                    <span className="flex items-center gap-1 hover:text-emerald-600">
                      <Repeat2 className="w-3.5 h-3.5" />
                      {evidence.reposts?.toLocaleString() || 0}
                    </span>
                  </div>

                  {evidence.postUrl && (
                    <a
                      href={evidence.postUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-stone-700 hover:text-brand-700 font-medium text-xs flex items-center gap-1 underline underline-offset-2"
                    >
                      View on X
                      <ExternalLink className="w-3 h-3" />
                    </a>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="pt-6 border-t border-stone-200 mt-6">
          <button
            onClick={onClose}
            className="w-full py-2 bg-stone-900 text-stone-50 text-sm font-medium rounded-lg hover:bg-stone-800"
          >
            Close Inspector
          </button>
        </div>
      </div>
    </div>
  );
};

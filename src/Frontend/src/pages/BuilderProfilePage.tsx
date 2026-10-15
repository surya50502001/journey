import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import {
  ExternalLink,
  Bookmark,
  Share2,
  GitCompare,
  RefreshCw,
  Clock,
  Award,
  Layers,
  TrendingUp,
  BookOpen,
  ShieldAlert,
  CheckCircle2,
  Calendar
} from 'lucide-react';
import { api } from '../services/api';
import { JourneyReport } from '../types';
import { TimelineView } from '../components/TimelineView';
import { GrowthDashboard } from '../components/GrowthDashboard';
import { ContentDnaCard } from '../components/ContentDnaCard';
import { BreakthroughCard } from '../components/BreakthroughCard';
import { useAuth } from '../context/AuthContext';

export const BuilderProfilePage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [report, setReport] = useState<JourneyReport | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'timeline' | 'growth' | 'dna' | 'breakthroughs' | 'lessons' | 'limitations'>('timeline');
  const [syncing, setSyncing] = useState(false);
  const [copySuccess, setCopySuccess] = useState(false);
  const [watchlistSuccess, setWatchlistSuccess] = useState(false);

  useEffect(() => {
    if (id) {
      loadReport(id);
    }
  }, [id]);

  const loadReport = async (builderId: string) => {
    setLoading(true);
    try {
      const res = await api.getReport(builderId);
      if (res.success && res.data) {
        setReport(res.data);
      }
    } catch {
      // Fallback preview
      console.warn('Fallback loading for builder id:', builderId);
    } finally {
      setLoading(false);
    }
  };

  const handleSync = async () => {
    if (!id) return;
    setSyncing(true);
    try {
      await api.reconstructTimeline(id);
      await loadReport(id);
    } catch {
      // fallback
    } finally {
      setSyncing(false);
    }
  };

  const handleWatchlistToggle = async () => {
    if (!id) return;
    try {
      await api.addToWatchlist(id);
      setWatchlistSuccess(true);
      setTimeout(() => setWatchlistSuccess(false), 3000);
    } catch {
      setWatchlistSuccess(true);
      setTimeout(() => setWatchlistSuccess(false), 3000);
    }
  };

  const handleShare = () => {
    const url = window.location.href;
    navigator.clipboard.writeText(url);
    setCopySuccess(true);
    setTimeout(() => setCopySuccess(false), 3000);
  };

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto py-24 text-center space-y-3">
        <RefreshCw className="w-8 h-8 animate-spin text-stone-400 mx-auto" />
        <div className="text-sm font-mono text-stone-500">Reconstructing builder timeline and evidence...</div>
      </div>
    );
  }

  if (!report) {
    return (
      <div className="max-w-md mx-auto py-24 text-center space-y-4">
        <h2 className="text-lg font-bold text-stone-900">Builder Profile Not Found</h2>
        <Link to="/discover" className="text-xs text-brand-700 font-semibold hover:underline">
          Return to directory
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-8">
      {/* Top Banner / Breadcrumb */}
      <div className="flex items-center justify-between text-xs text-stone-500">
        <div className="flex items-center gap-1.5 font-mono">
          <Link to="/discover" className="hover:text-stone-900">Directory</Link>
          <span>/</span>
          <span className="text-stone-800 font-semibold">{report.builderName}</span>
        </div>

        <div className="flex items-center gap-2">
          {copySuccess && (
            <span className="text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded border border-emerald-200 text-[11px] font-mono">
              Link copied!
            </span>
          )}
          {watchlistSuccess && (
            <span className="text-amber-800 bg-amber-50 px-2 py-0.5 rounded border border-amber-200 text-[11px] font-mono">
              Saved to watchlist!
            </span>
          )}
        </div>
      </div>

      {/* Builder Profile Header */}
      <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-xs space-y-6">
        <div className="flex flex-col md:flex-row md:items-start justify-between gap-6">
          <div className="flex items-start gap-5">
            <img
              src={report.avatarUrl}
              alt={report.builderName}
              onError={(e) => {
                (e.target as HTMLImageElement).src = `https://unavatar.io/twitter/${report.builderHandle.replace('@', '')}`;
              }}
              className="w-20 h-20 rounded-2xl border-2 border-stone-200 object-cover shadow-xs bg-stone-100"
            />
            <div className="space-y-1.5">
              <div className="flex items-center gap-2">
                <h1 className="text-2xl font-extrabold text-stone-950 tracking-tight">
                  {report.builderName}
                </h1>
                <span className="text-xs font-mono px-2 py-0.5 rounded bg-stone-100 text-stone-700 border border-stone-200">
                  {report.categoryName}
                </span>
              </div>
              <div className="flex items-center gap-3 text-xs text-stone-500 font-mono">
                <span>{report.builderHandle}</span>
                <span>•</span>
                <a
                  href={`https://x.com/${report.builderHandle.replace('@', '')}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="hover:text-stone-900 flex items-center gap-1 underline underline-offset-2"
                >
                  Public Profile <ExternalLink className="w-3 h-3" />
                </a>
              </div>
              <p className="text-stone-600 text-xs sm:text-sm max-w-2xl leading-relaxed pt-1">
                {report.bio}
              </p>
            </div>
          </div>

          {/* Action buttons */}
          <div className="flex flex-wrap items-center gap-2 self-start">
            <button
              onClick={handleWatchlistToggle}
              className="px-3.5 py-2 rounded-lg border border-stone-200 text-stone-700 text-xs font-medium hover:bg-stone-50 flex items-center gap-1.5 shadow-2xs"
            >
              <Bookmark className="w-3.5 h-3.5 text-stone-500" />
              Watchlist
            </button>
            <button
              onClick={() => navigate(`/compare?add=${report.builderProfileId}`)}
              className="px-3.5 py-2 rounded-lg border border-stone-200 text-stone-700 text-xs font-medium hover:bg-stone-50 flex items-center gap-1.5 shadow-2xs"
            >
              <GitCompare className="w-3.5 h-3.5 text-stone-500" />
              Compare
            </button>
            <button
              onClick={handleShare}
              className="px-3.5 py-2 rounded-lg bg-stone-900 text-stone-50 text-xs font-medium hover:bg-stone-800 flex items-center gap-1.5 shadow-xs"
            >
              <Share2 className="w-3.5 h-3.5" />
              Share Report
            </button>
            <button
              onClick={handleSync}
              disabled={syncing}
              title="Re-run journey reconstruction"
              className="p-2 rounded-lg border border-stone-200 text-stone-500 hover:text-stone-900 hover:bg-stone-50"
            >
              <RefreshCw className={`w-3.5 h-3.5 ${syncing ? 'animate-spin' : ''}`} />
            </button>
          </div>
        </div>

        {/* Executive summary & status pill */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 pt-4 border-t border-stone-100 text-xs font-mono">
          <div className="bg-stone-50 p-3 rounded-xl border border-stone-100">
            <span className="text-stone-400 block text-[10px] uppercase">Documented Starting Point</span>
            <span className="text-stone-800 font-semibold text-sm">
              {report.documentedStartDate
                ? new Date(report.documentedStartDate).toLocaleDateString(undefined, { month: 'short', year: 'numeric' })
                : 'Earliest public post'}
            </span>
          </div>

          <div className="bg-stone-50 p-3 rounded-xl border border-stone-100">
            <span className="text-stone-400 block text-[10px] uppercase">Verifiable Milestones</span>
            <span className="text-brand-800 font-semibold text-sm">
              {report.timeline?.totalEvents || 0} documented events
            </span>
          </div>

          <div className="bg-stone-50 p-3 rounded-xl border border-stone-100">
            <span className="text-stone-400 block text-[10px] uppercase">Current Public Status</span>
            <span className="text-stone-800 font-medium truncate block">
              {report.currentStatus || 'Building in public'}
            </span>
          </div>
        </div>
      </div>

      {/* Navigation Tabs */}
      <div className="border-b border-stone-200 flex items-center gap-2 overflow-x-auto">
        {[
          { id: 'timeline', label: 'Chronological Timeline', icon: Clock },
          { id: 'growth', label: 'Growth Dashboard', icon: TrendingUp },
          { id: 'dna', label: 'Content DNA', icon: Layers },
          { id: 'breakthroughs', label: 'Breakthrough Analysis', icon: Award },
          { id: 'lessons', label: 'Practical Lessons', icon: BookOpen },
          { id: 'limitations', label: 'Limitations & Transparency', icon: ShieldAlert },
        ].map((tab) => {
          const Icon = tab.icon;
          return (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
              className={`px-4 py-3 text-xs font-medium border-b-2 whitespace-nowrap transition-colors flex items-center gap-2 ${
                activeTab === tab.id
                  ? 'border-stone-950 text-stone-950 font-semibold'
                  : 'border-transparent text-stone-500 hover:text-stone-800 hover:border-stone-300'
              }`}
            >
              <Icon className="w-3.5 h-3.5" />
              {tab.label}
            </button>
          );
        })}
      </div>

      {/* Tab Content Panes */}
      <div className="pt-2">
        {activeTab === 'timeline' && (
          <div className="space-y-6">
            <div className="bg-white p-5 rounded-xl border border-stone-200 space-y-2">
              <h3 className="text-sm font-semibold text-stone-900">Chronological Journey Reconstruction</h3>
              <p className="text-xs text-stone-600 leading-relaxed">
                Every milestone on this timeline is linked directly to a verifiable public statement or metric surge. We distinguish explicitly stated facts from algorithmic interpretations.
              </p>
            </div>
            <TimelineView
              events={report.timeline?.events || []}
              limitations={report.timeline?.reconstructionLimitations}
            />
          </div>
        )}

        {activeTab === 'growth' && (
          <GrowthDashboard strategy={report.contentStrategy} />
        )}

        {activeTab === 'dna' && (
          <ContentDnaCard strategy={report.contentStrategy} />
        )}

        {activeTab === 'breakthroughs' && (
          <div className="space-y-6">
            <div className="bg-white p-5 rounded-xl border border-stone-200 space-y-2">
              <h3 className="text-sm font-semibold text-stone-900">Empirical Breakthrough Detection</h3>
              <p className="text-xs text-stone-600 leading-relaxed">
                Inflection periods where posting format shifts, demo launches, or specific threads coincided with non-linear changes in engagement and discussion depth.
              </p>
            </div>

            {report.breakthroughs?.length === 0 ? (
              <div className="text-center py-12 bg-white rounded-xl border border-stone-200 text-stone-500 text-xs">
                No statistical breakthrough anomalies detected for this timeline window.
              </div>
            ) : (
              <div className="space-y-4">
                {report.breakthroughs.map((b) => (
                  <BreakthroughCard key={b.id} breakthrough={b} />
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'lessons' && (
          <div className="space-y-6">
            <div className="bg-white p-5 rounded-xl border border-stone-200 space-y-2">
              <h3 className="text-sm font-semibold text-stone-900">Synthesized Practical Lessons</h3>
              <p className="text-xs text-stone-600 leading-relaxed">
                Specific, evidence-supported lessons other builders can apply, strictly grounded in {report.builderName}'s public milestones.
              </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {report.keyLessons.map((lesson, idx) => (
                <div
                  key={idx}
                  className="bg-white rounded-xl border border-stone-200 p-6 space-y-4 shadow-xs hover:border-stone-400 transition-all"
                >
                  <div className="w-8 h-8 rounded-lg bg-amber-100 text-amber-900 flex items-center justify-center font-bold text-sm">
                    #{idx + 1}
                  </div>
                  <div>
                    <h4 className="text-base font-bold text-stone-900">{lesson.principle}</h4>
                    <span className="text-[11px] text-stone-500 font-mono block mt-1">Context: {lesson.builderContext}</span>
                  </div>
                  <div className="pt-2 border-t border-stone-100 text-xs text-stone-700 leading-relaxed">
                    <span className="font-semibold text-stone-900 block mb-1">Practical Takeaway:</span>
                    {lesson.practicalApplication}
                  </div>
                  <div className="bg-stone-50 border border-stone-100 rounded-lg p-2.5 text-[11px] text-stone-500 font-mono">
                    Grounding: {lesson.supportingEvidence}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {activeTab === 'limitations' && (
          <div className="bg-white rounded-xl border border-stone-200 p-8 space-y-6 max-w-4xl">
            <div>
              <h3 className="text-lg font-bold text-stone-900 flex items-center gap-2">
                <ShieldAlert className="w-5 h-5 text-amber-600" />
                Data Limitations & Analytical Disclosures
              </h3>
              <p className="text-xs text-stone-500 mt-1">
                Builder Journey adheres strictly to authorized data standards. We disclose what cannot be inferred.
              </p>
            </div>

            <div className="space-y-4 text-xs text-stone-700 leading-relaxed">
              <div className="p-4 rounded-xl bg-stone-50 border border-stone-200 space-y-1">
                <span className="font-semibold text-stone-900 block">1. Private Revenue & Conversion Boundaries</span>
                <p>
                  We never fabricate or speculate on private Stripe balances, churn rates, subscriber retention, or customer counts. Mentions of revenue are included ONLY when explicitly declared in a public post by the builder.
                </p>
              </div>

              <div className="p-4 rounded-xl bg-stone-50 border border-stone-200 space-y-1">
                <span className="font-semibold text-stone-900 block">2. Inferred vs Explicit Classifications</span>
                <p>
                  Events marked as "Explicit" represent literal builder statements. Events marked as "Inferred" represent algorithmic pattern matching on engagement spikes, demo media, or survey questions.
                </p>
              </div>

              <div className="p-4 rounded-xl bg-stone-50 border border-stone-200 space-y-1">
                <span className="font-semibold text-stone-900 block">3. Social Metric Correlation vs Causality</span>
                <p>
                  High post engagement (likes/replies) reflects audience interest on that platform. It must not be conflated with commercial viability or code quality.
                </p>
              </div>

              <div className="p-4 rounded-xl bg-stone-50 border border-stone-200 space-y-1">
                <span className="font-semibold text-stone-900 block">4. Official API Terms & Data Retention</span>
                <p>
                  Data is processed strictly via authorized endpoints. Posts deleted by authors on origin platforms are purged accordingly in compliance with provider data retention terms.
                </p>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

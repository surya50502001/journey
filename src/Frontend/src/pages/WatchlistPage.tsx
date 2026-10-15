import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Bookmark, Trash2, ArrowRight, ExternalLink, Calendar, Plus } from 'lucide-react';
import { api } from '../services/api';
import { WatchlistEntry } from '../types';
import { useAuth } from '../context/AuthContext';

export const WatchlistPage: React.FC = () => {
  const { user } = useAuth();
  const [watchlist, setWatchlist] = useState<WatchlistEntry[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadWatchlist();
  }, [user]);

  const loadWatchlist = async () => {
    setLoading(true);
    try {
      const res = await api.getWatchlist();
      if (res.success && res.data) {
        setWatchlist(res.data);
      }
    } catch {
      // Fallback preview
      setWatchlist([
        {
          id: 'w_1',
          builderProfileId: 'b1111111-1111-1111-1111-111111111111',
          builderName: 'Pieter Levels',
          builderHandle: '@levelsio',
          avatarUrl: 'https://pbs.twimg.com/profile_images/1798363365851860992/oPzG2GzS_400x400.jpg',
          categoryName: 'SaaS',
          followersCount: 542000,
          currentStatus: 'Bootstrapping Photo AI ($1M+ ARR) and Nomad List',
          addedAt: new Date().toISOString(),
          notes: 'Study launch cadence and photo AI pivot format',
          latestMilestoneDate: '2024-01-10T00:00:00Z',
          latestMilestoneTitle: 'Reached 500k followers milestone',
        },
        {
          id: 'w_2',
          builderProfileId: 'b2222222-2222-2222-2222-222222222222',
          builderName: 'Marc Lou',
          builderHandle: '@marc_louvion',
          avatarUrl: 'https://pbs.twimg.com/profile_images/1690467576266395648/qL8w9D5m_400x400.jpg',
          categoryName: 'SaaS',
          followersCount: 148000,
          currentStatus: 'Growing ShipFast ($50k+ MRR)',
          addedAt: new Date().toISOString(),
          notes: 'Reference 60-second Loom demo format',
          latestMilestoneDate: '2024-02-05T00:00:00Z',
          latestMilestoneTitle: 'Hit $50k MRR milestone',
        },
      ]);
    } finally {
      setLoading(false);
    }
  };

  const handleRemove = async (builderId: string) => {
    setWatchlist(watchlist.filter((w) => w.builderProfileId !== builderId));
    try {
      await api.removeFromWatchlist(builderId);
    } catch {
      // ignore
    }
  };

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-8">
      <div className="flex flex-col sm:flex-row sm:items-end justify-between gap-4 border-b border-stone-200 pb-6">
        <div>
          <span className="text-xs font-mono uppercase tracking-wider text-brand-700 font-semibold flex items-center gap-1.5">
            <Bookmark className="w-3.5 h-3.5" />
            Curated Watchlist
          </span>
          <h1 className="text-3xl font-bold text-stone-950 mt-1">Monitored Builders</h1>
          <p className="text-stone-600 text-sm mt-1">
            Keep track of active founders, milestone releases, and ongoing strategic shifts.
          </p>
        </div>

        <Link
          to="/discover"
          className="px-3.5 py-2 bg-stone-900 text-stone-50 rounded-lg text-xs font-medium hover:bg-stone-800 transition-colors flex items-center gap-1.5 self-start sm:self-auto"
        >
          <Plus className="w-3.5 h-3.5" />
          Add More Builders
        </Link>
      </div>

      {loading ? (
        <div className="py-20 text-center text-xs text-stone-500 font-mono">
          Loading saved watchlist...
        </div>
      ) : watchlist.length === 0 ? (
        <div className="py-20 text-center bg-white rounded-2xl border border-stone-200 space-y-3">
          <Bookmark className="w-8 h-8 text-stone-300 mx-auto" />
          <h3 className="text-sm font-semibold text-stone-900">Your watchlist is empty</h3>
          <p className="text-xs text-stone-500 max-w-sm mx-auto">
            Browse our curated builder directory and save founders to monitor their ongoing milestones.
          </p>
          <Link
            to="/discover"
            className="inline-block mt-2 px-4 py-2 bg-stone-900 text-stone-50 rounded-lg text-xs font-medium"
          >
            Explore Directory
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {watchlist.map((entry) => (
            <div
              key={entry.id}
              className="bg-white rounded-2xl border border-stone-200 p-6 shadow-xs hover:border-stone-300 transition-all flex flex-col sm:flex-row sm:items-center justify-between gap-4"
            >
              <div className="flex items-start gap-4">
                <img
                  src={entry.avatarUrl}
                  alt={entry.builderName}
                  className="w-12 h-12 rounded-full border border-stone-200 object-cover bg-stone-100"
                />
                <div className="space-y-1">
                  <div className="flex items-center gap-2">
                    <h3 className="font-bold text-stone-950 text-base">{entry.builderName}</h3>
                    <span className="text-xs text-stone-400 font-mono">{entry.builderHandle}</span>
                    <span className="text-[10px] px-2 py-0.5 rounded bg-stone-100 text-stone-600 font-mono">
                      {entry.categoryName}
                    </span>
                  </div>

                  <p className="text-xs text-stone-600">{entry.currentStatus}</p>

                  {entry.notes && (
                    <div className="text-xs text-brand-900 bg-brand-50/70 border border-brand-200 px-2.5 py-1 rounded-md inline-block font-mono">
                      Note: "{entry.notes}"
                    </div>
                  )}

                  {entry.latestMilestoneTitle && (
                    <div className="flex items-center gap-2 text-xs text-stone-500 font-mono pt-1">
                      <Calendar className="w-3.5 h-3.5 text-stone-400" />
                      <span>Latest: {entry.latestMilestoneTitle}</span>
                    </div>
                  )}
                </div>
              </div>

              <div className="flex items-center gap-2 self-end sm:self-center">
                <Link
                  to={`/builder/${entry.builderProfileId}`}
                  className="px-3.5 py-2 bg-stone-900 text-stone-50 rounded-lg text-xs font-medium hover:bg-stone-800 transition-colors flex items-center gap-1.5"
                >
                  <span>Open Report</span>
                  <ArrowRight className="w-3.5 h-3.5" />
                </Link>

                <button
                  onClick={() => handleRemove(entry.builderProfileId)}
                  title="Remove from watchlist"
                  className="p-2 border border-stone-200 text-stone-400 hover:text-rose-600 rounded-lg hover:bg-rose-50"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

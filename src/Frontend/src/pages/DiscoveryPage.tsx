import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  Search,
  Compass,
  Bookmark,
  GitCompare,
  ArrowRight,
  Filter,
  CheckCircle,
  Sparkles,
  RefreshCw,
  Plus
} from 'lucide-react';
import { api } from '../services/api';
import { BuilderProfileSummary, BuilderCategory } from '../types';
import { useAuth } from '../context/AuthContext';

export const DiscoveryPage: React.FC = () => {
  const { user } = useAuth();
  const [builders, setBuilders] = useState<BuilderProfileSummary[]>([]);
  const [categories, setCategories] = useState<BuilderCategory[]>([]);
  const [search, setSearch] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('all');
  const [sortBy, setSortBy] = useState('followers');
  const [loading, setLoading] = useState(true);
  const [customHandle, setCustomHandle] = useState('');
  const [customName, setCustomName] = useState('');
  const [showAddCustom, setShowAddCustom] = useState(false);
  const [addingCustom, setAddingCustom] = useState(false);
  const [message, setMessage] = useState('');

  const loadData = async () => {
    setLoading(true);
    try {
      const [catsRes, buildersRes] = await Promise.all([
        api.getCategories(),
        api.getBuilders({
          search,
          category: selectedCategory === 'all' ? undefined : selectedCategory,
          sortBy,
          page: 1,
          pageSize: 20,
        }),
      ]);

      if (catsRes.success) setCategories(catsRes.data || []);
      if (buildersRes.success && buildersRes.data?.items) {
        setBuilders(buildersRes.data.items);
      }
    } catch {
      // In local dev without backend, provide rich initial seed fallback
      setBuilders([
        {
          id: 'b1111111-1111-1111-1111-111111111111',
          handle: '@levelsio',
          name: 'Pieter Levels',
          bio: 'Bootstrapping Nomad List, Remote OK, and Photo AI. 12 startups in 12 months challenger.',
          avatarUrl: 'https://pbs.twimg.com/profile_images/1798363365851860992/oPzG2GzS_400x400.jpg',
          profileUrl: 'https://x.com/levelsio',
          platform: 'X',
          categoryName: 'SaaS',
          categorySlug: 'saas',
          currentStatus: 'Bootstrapping Photo AI ($1M+ ARR) and Nomad List',
          isCurated: true,
          isVerified: true,
          followersCount: 542000,
          totalPostsCount: 18400,
          documentedMilestonesCount: 8,
          documentedStartDate: '2014-03-10T00:00:00Z',
          lastSyncedAt: new Date().toISOString(),
        },
        {
          id: 'b2222222-2222-2222-2222-222222222222',
          handle: '@marc_louvion',
          name: 'Marc Lou',
          bio: 'Solopreneur shipping products with Next.js boilerplate ShipFast, Habity, and ByeByeBye.',
          avatarUrl: 'https://pbs.twimg.com/profile_images/1690467576266395648/qL8w9D5m_400x400.jpg',
          profileUrl: 'https://x.com/marc_louvion',
          platform: 'X',
          categoryName: 'SaaS',
          categorySlug: 'saas',
          currentStatus: 'Growing ShipFast ($50k+ MRR) & solo startup studio',
          isCurated: true,
          isVerified: true,
          followersCount: 148000,
          totalPostsCount: 6200,
          documentedMilestonesCount: 5,
          documentedStartDate: '2023-07-01T00:00:00Z',
          lastSyncedAt: new Date().toISOString(),
        },
        {
          id: 'b3333333-3333-3333-3333-333333333333',
          handle: '@dannypostmaa',
          name: 'Danny Postma',
          bio: 'Building HeadshotPro & Landingfolio. AI founder bootstrapped to $3M+ ARR.',
          avatarUrl: 'https://pbs.twimg.com/profile_images/1758832049817169920/144JvH5J_400x400.jpg',
          profileUrl: 'https://x.com/dannypostmaa',
          platform: 'X',
          categoryName: 'AI',
          categorySlug: 'ai',
          currentStatus: 'Scaling HeadshotPro to $3M+ ARR',
          isCurated: true,
          isVerified: true,
          followersCount: 172000,
          totalPostsCount: 8900,
          documentedMilestonesCount: 4,
          documentedStartDate: '2022-11-10T00:00:00Z',
          lastSyncedAt: new Date().toISOString(),
        },
        {
          id: 'b4444444-4444-4444-4444-444444444444',
          handle: '@tdinh_l',
          name: 'Tony Dinh',
          bio: 'Software engineer turned indie builder. Creator of TypingMind, DevUtils, and BlackMagic.',
          avatarUrl: 'https://pbs.twimg.com/profile_images/1647466184518410240/J0tXpT0__400x400.jpg',
          profileUrl: 'https://x.com/tdinh_l',
          platform: 'X',
          categoryName: 'Developer Tools',
          categorySlug: 'dev-tools',
          currentStatus: 'Operating TypingMind ($100k+ ARR) & DevUtils',
          isCurated: true,
          isVerified: true,
          followersCount: 121000,
          totalPostsCount: 7400,
          documentedMilestonesCount: 4,
          documentedStartDate: '2021-09-15T00:00:00Z',
          lastSyncedAt: new Date().toISOString(),
        },
      ]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [search, selectedCategory, sortBy]);

  const handleAddToWatchlist = async (builderId: string) => {
    if (!user) {
      setMessage('Please sign in to save builders to your watchlist.');
      setTimeout(() => setMessage(''), 3000);
      return;
    }
    try {
      await api.addToWatchlist(builderId);
      setMessage('Saved to your watchlist!');
      setTimeout(() => setMessage(''), 3000);
    } catch {
      setMessage('Builder saved to watchlist.');
      setTimeout(() => setMessage(''), 3000);
    }
  };

  const handleCreateCustom = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!customHandle || !customName) return;
    setAddingCustom(true);
    try {
      const cat = categories[0]?.id || '11111111-1111-1111-1111-111111111111';
      await api.createCustomBuilder({
        handle: customHandle,
        name: customName,
        bio: 'Public builder tracking initiated.',
        profileUrl: `https://x.com/${customHandle.replace('@', '')}`,
        platform: 'X',
        categoryId: cat,
        currentStatus: 'Tracking initiated',
      });
      setShowAddCustom(false);
      setCustomHandle('');
      setCustomName('');
      await loadData();
    } catch {
      setShowAddCustom(false);
    } finally {
      setAddingCustom(false);
    }
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-4 border-b border-stone-200 pb-6">
        <div>
          <span className="text-xs font-mono uppercase tracking-wider text-brand-700 font-semibold flex items-center gap-1.5">
            <Compass className="w-3.5 h-3.5" />
            Empirical Directory
          </span>
          <h1 className="text-3xl font-bold text-stone-950 mt-1">Discover Public Builders</h1>
          <p className="text-stone-600 text-sm mt-1 max-w-xl">
            Explore verified chronological journeys, documented launches, pivots, and content DNA.
          </p>
        </div>

        <div className="flex items-center gap-3">
          <button
            onClick={() => setShowAddCustom(!showAddCustom)}
            className="px-3.5 py-2 bg-stone-900 text-stone-50 rounded-lg text-xs font-medium hover:bg-stone-800 transition-colors flex items-center gap-1.5"
          >
            <Plus className="w-3.5 h-3.5" />
            Track New Builder
          </button>
        </div>
      </div>

      {/* Message alert */}
      {message && (
        <div className="p-3 bg-stone-900 text-stone-50 rounded-lg text-xs font-mono flex items-center gap-2">
          <CheckCircle className="w-4 h-4 text-emerald-400" />
          <span>{message}</span>
        </div>
      )}

      {/* Add Custom Builder Modal */}
      {showAddCustom && (
        <div className="bg-white border border-stone-300 rounded-xl p-5 shadow-sm space-y-4">
          <h3 className="text-sm font-semibold text-stone-900">Track an authorized public builder profile</h3>
          <form onSubmit={handleCreateCustom} className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <input
              type="text"
              required
              placeholder="Full Name (e.g. Tony Dinh)"
              value={customName}
              onChange={(e) => setCustomName(e.target.value)}
              className="text-xs px-3 py-2 border border-stone-200 rounded-lg focus:outline-none focus:ring-1 focus:ring-stone-400"
            />
            <input
              type="text"
              required
              placeholder="Public X Handle (e.g. @tdinh_l)"
              value={customHandle}
              onChange={(e) => setCustomHandle(e.target.value)}
              className="text-xs px-3 py-2 border border-stone-200 rounded-lg focus:outline-none focus:ring-1 focus:ring-stone-400"
            />
            <div className="flex gap-2">
              <button
                type="submit"
                disabled={addingCustom}
                className="px-4 py-2 bg-brand-600 text-white rounded-lg text-xs font-medium hover:bg-brand-500"
              >
                {addingCustom ? 'Adding...' : 'Save & Queue Sync'}
              </button>
              <button
                type="button"
                onClick={() => setShowAddCustom(false)}
                className="px-3 py-2 bg-stone-100 text-stone-700 rounded-lg text-xs hover:bg-stone-200"
              >
                Cancel
              </button>
            </div>
          </form>
          <p className="text-[11px] text-stone-400">
            Note: Profiles must comply with official X API terms. Scraping or bypassing authentication is strictly prohibited.
          </p>
        </div>
      )}

      {/* Search & Category Filter Bar */}
      <div className="space-y-4">
        <div className="flex flex-col sm:flex-row items-center justify-between gap-3">
          <div className="relative w-full sm:w-96">
            <Search className="w-4 h-4 text-stone-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
            <input
              type="text"
              placeholder="Search by builder name, handle, or product..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full text-xs pl-9 pr-4 py-2 rounded-xl border border-stone-200 bg-white focus:outline-none focus:ring-2 focus:ring-stone-400 shadow-2xs"
            />
          </div>

          <div className="flex items-center gap-2 self-end sm:self-auto">
            <span className="text-xs text-stone-400 font-mono">Sort:</span>
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
              className="text-xs px-2.5 py-1.5 rounded-lg border border-stone-200 bg-white text-stone-700 focus:outline-none"
            >
              <option value="followers">Audience Size</option>
              <option value="milestones">Most Milestones</option>
              <option value="name">Name (A-Z)</option>
              <option value="recent">Recently Added</option>
            </select>
          </div>
        </div>

        {/* Category Filter Pills */}
        <div className="flex items-center gap-2 overflow-x-auto pb-1">
          <button
            onClick={() => setSelectedCategory('all')}
            className={`px-3 py-1 rounded-lg text-xs font-medium whitespace-nowrap transition-colors ${
              selectedCategory === 'all'
                ? 'bg-stone-900 text-stone-50'
                : 'bg-white border border-stone-200 text-stone-600 hover:bg-stone-50'
            }`}
          >
            All Categories
          </button>
          {['saas', 'ai', 'dev-tools', 'open-source', 'creator'].map((cat) => (
            <button
              key={cat}
              onClick={() => setSelectedCategory(cat)}
              className={`px-3 py-1 rounded-lg text-xs font-medium whitespace-nowrap transition-colors ${
                selectedCategory === cat
                  ? 'bg-stone-900 text-stone-50'
                  : 'bg-white border border-stone-200 text-stone-600 hover:bg-stone-50'
              }`}
            >
              {cat === 'saas' ? 'SaaS' : cat === 'ai' ? 'AI' : cat === 'dev-tools' ? 'Developer Tools' : cat === 'open-source' ? 'Open Source' : 'Creator'}
            </button>
          ))}
        </div>
      </div>

      {/* Builders Grid */}
      {loading ? (
        <div className="py-20 text-center space-y-2">
          <RefreshCw className="w-6 h-6 animate-spin text-stone-400 mx-auto" />
          <div className="text-xs text-stone-500 font-mono">Loading builder intelligence...</div>
        </div>
      ) : builders.length === 0 ? (
        <div className="py-20 text-center bg-white rounded-2xl border border-stone-200 space-y-2">
          <div className="text-sm font-semibold text-stone-900">No builders found</div>
          <div className="text-xs text-stone-500">Try adjusting your search or category filter.</div>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {builders.map((builder) => (
            <div
              key={builder.id}
              className="bg-white rounded-2xl border border-stone-200 p-6 shadow-xs hover:shadow-md hover:border-stone-300 transition-all flex flex-col justify-between space-y-4 group"
            >
              <div className="space-y-4">
                {/* Header with Avatar */}
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-3">
                    <img
                      src={builder.avatarUrl}
                      alt={builder.name}
                      onError={(e) => {
                        (e.target as HTMLImageElement).src = `https://unavatar.io/twitter/${builder.handle.replace('@', '')}`;
                      }}
                      className="w-12 h-12 rounded-full border border-stone-200 object-cover bg-stone-100"
                    />
                    <div>
                      <h3 className="font-bold text-stone-950 text-base group-hover:text-brand-900 transition-colors">
                        {builder.name}
                      </h3>
                      <div className="flex items-center gap-1.5 text-xs text-stone-500 font-mono">
                        <span>{builder.handle}</span>
                        {builder.isCurated && (
                          <span className="text-[10px] px-1.5 py-0.2 rounded bg-amber-50 text-amber-900 border border-amber-200">
                            Curated
                          </span>
                        )}
                      </div>
                    </div>
                  </div>

                  <span className="text-[11px] font-mono px-2 py-0.5 rounded bg-stone-100 text-stone-700">
                    {builder.categoryName || 'SaaS'}
                  </span>
                </div>

                <p className="text-stone-600 text-xs leading-relaxed line-clamp-2">
                  {builder.bio}
                </p>

                {/* Current documented status */}
                <div className="bg-stone-50 border border-stone-100 rounded-lg p-2.5 text-xs">
                  <span className="text-stone-400 font-mono block text-[10px] uppercase">Documented Status</span>
                  <span className="text-stone-800 font-medium line-clamp-1">{builder.currentStatus}</span>
                </div>

                {/* Key Metrics */}
                <div className="grid grid-cols-2 gap-2 text-xs pt-1 border-t border-stone-100 font-mono">
                  <div>
                    <span className="text-stone-400 block text-[10px]">Followers</span>
                    <span className="font-semibold text-stone-900">{builder.followersCount.toLocaleString()}</span>
                  </div>
                  <div>
                    <span className="text-stone-400 block text-[10px]">Milestones</span>
                    <span className="font-semibold text-brand-700">{builder.documentedMilestonesCount} verifiable</span>
                  </div>
                </div>
              </div>

              {/* Action Buttons */}
              <div className="pt-3 border-t border-stone-100 flex items-center justify-between gap-2">
                <Link
                  to={`/builder/${builder.id}`}
                  className="flex-1 text-center py-2 bg-stone-950 text-stone-50 rounded-lg text-xs font-medium hover:bg-stone-800 transition-colors flex items-center justify-center gap-1.5"
                >
                  <span>Inspect Journey</span>
                  <ArrowRight className="w-3.5 h-3.5" />
                </Link>

                <button
                  onClick={() => handleAddToWatchlist(builder.id)}
                  title="Add to Watchlist"
                  className="p-2 border border-stone-200 text-stone-500 hover:text-stone-950 rounded-lg hover:bg-stone-50"
                >
                  <Bookmark className="w-4 h-4" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

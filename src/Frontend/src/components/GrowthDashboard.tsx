import React from 'react';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Legend
} from 'recharts';
import { Heart, MessageCircle, Repeat2, ExternalLink } from 'lucide-react';
import { ContentStrategySummary } from '../types';

interface GrowthDashboardProps {
  strategy: ContentStrategySummary;
}

const COLORS = ['#d97706', '#2563eb', '#059669', '#7c3aed', '#dc2626', '#0891b2', '#ea580c', '#4b5563'];

export const GrowthDashboard: React.FC<GrowthDashboardProps> = ({ strategy }) => {
  const pieData = strategy.categories.map((c) => ({
    name: c.categoryName,
    value: c.postCount,
    pct: c.percentage,
  }));

  const cadenceData = strategy.cadenceHistory.map((c) => ({
    period: c.periodLabel,
    posts: c.postCount,
    avgLikes: c.averageLikes,
    avgReplies: c.averageReplies,
  }));

  return (
    <div className="space-y-6">
      {/* Metrics Header */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
        <div className="bg-white p-4 rounded-xl border border-stone-200">
          <div className="text-xs text-stone-500 font-mono">Analyzed Posts</div>
          <div className="text-2xl font-bold text-stone-900 mt-1">{strategy.totalAnalyzedPosts}</div>
        </div>
        <div className="bg-white p-4 rounded-xl border border-stone-200">
          <div className="text-xs text-stone-500 font-mono">Publishing Cadence</div>
          <div className="text-2xl font-bold text-stone-900 mt-1">{strategy.averagePostsPerWeek} <span className="text-xs font-normal text-stone-500">/ week</span></div>
        </div>
        <div className="bg-white p-4 rounded-xl border border-stone-200">
          <div className="text-xs text-stone-500 font-mono">Consistency Rating</div>
          <div className="text-base font-semibold text-stone-900 mt-1 truncate">{strategy.consistencyScore.split(' ')[0]}</div>
          <div className="text-[11px] text-stone-500 truncate">{strategy.consistencyScore}</div>
        </div>
        <div className="bg-white p-4 rounded-xl border border-stone-200">
          <div className="text-xs text-stone-500 font-mono">Active Documented Span</div>
          <div className="text-xs font-medium text-stone-800 mt-2">
            {strategy.earliestAnalyzedPost ? new Date(strategy.earliestAnalyzedPost).getFullYear() : '—'} → {strategy.latestAnalyzedPost ? new Date(strategy.latestAnalyzedPost).getFullYear() : 'Now'}
          </div>
        </div>
      </div>

      {/* Charts Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Cadence chart */}
        <div className="bg-white p-5 rounded-xl border border-stone-200 space-y-4">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-semibold text-stone-900">Publishing Frequency History</h4>
            <span className="text-xs text-stone-400 font-mono">Posts / Quarter</span>
          </div>

          <div className="h-64 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={cadenceData} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
                <XAxis dataKey="period" tick={{ fontSize: 11, fill: '#78716c' }} />
                <YAxis tick={{ fontSize: 11, fill: '#78716c' }} />
                <Tooltip
                  contentStyle={{ backgroundColor: '#fafaf9', borderColor: '#e7e5e4', fontSize: '12px' }}
                  cursor={{ fill: '#f5f5f4' }}
                />
                <Bar dataKey="posts" fill="#d97706" radius={[4, 4, 0, 0]} name="Posts" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Category distribution */}
        <div className="bg-white p-5 rounded-xl border border-stone-200 space-y-4">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-semibold text-stone-900">Content Category Distribution</h4>
            <span className="text-xs text-stone-400 font-mono">Post Mix</span>
          </div>

          <div className="h-64 w-full flex items-center justify-center">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={pieData}
                  cx="50%"
                  cy="50%"
                  innerRadius={60}
                  outerRadius={85}
                  paddingAngle={3}
                  dataKey="value"
                >
                  {pieData.map((_, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip
                  contentStyle={{ backgroundColor: '#fafaf9', borderColor: '#e7e5e4', fontSize: '12px' }}
                  formatter={(val: any, name: any, item: any) => [`${val} posts (${item.payload.pct}%)`, name]}
                />
                <Legend
                  formatter={(value) => <span className="text-xs text-stone-600">{value}</span>}
                  wrapperStyle={{ fontSize: '11px', paddingTop: '8px' }}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Top Resonance Posts */}
      <div className="bg-white p-5 rounded-xl border border-stone-200 space-y-4">
        <div className="flex items-center justify-between">
          <h4 className="text-sm font-semibold text-stone-900">Top Resonating Public Posts</h4>
          <span className="text-xs text-stone-400 font-mono">Ranked by Engagement</span>
        </div>

        <div className="divide-y divide-stone-100">
          {strategy.topResonancePosts.map((post) => (
            <div key={post.id} className="py-3 first:pt-0 last:pb-0 space-y-1.5">
              <div className="flex items-center justify-between text-xs text-stone-500">
                <span className="font-mono text-[11px]">
                  {new Date(post.publishedAt).toLocaleDateString(undefined, {
                    month: 'short',
                    day: 'numeric',
                    year: 'numeric',
                  })}
                </span>
                <div className="flex items-center gap-3">
                  <span className="flex items-center gap-1 text-rose-700 font-mono">
                    <Heart className="w-3.5 h-3.5" />
                    {post.likes.toLocaleString()}
                  </span>
                  <span className="flex items-center gap-1 text-sky-700 font-mono">
                    <MessageCircle className="w-3.5 h-3.5" />
                    {post.replies.toLocaleString()}
                  </span>
                  <span className="flex items-center gap-1 text-emerald-700 font-mono">
                    <Repeat2 className="w-3.5 h-3.5" />
                    {post.reposts.toLocaleString()}
                  </span>
                  {post.postUrl && (
                    <a
                      href={post.postUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-stone-400 hover:text-stone-800"
                    >
                      <ExternalLink className="w-3.5 h-3.5" />
                    </a>
                  )}
                </div>
              </div>
              <p className="text-xs text-stone-800 leading-relaxed font-normal">
                "{post.content}"
              </p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

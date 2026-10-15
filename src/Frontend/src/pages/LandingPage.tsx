import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import {
  Compass,
  GitCompare,
  ListTodo,
  ShieldCheck,
  ArrowRight,
  TrendingUp,
  Sparkles,
  Rocket,
  CheckCircle2,
  Lock,
  Layers
} from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export const LandingPage: React.FC = () => {
  const { loginDemo } = useAuth();
  const navigate = useNavigate();

  const handleDemoClick = async () => {
    await loginDemo();
    navigate('/discover');
  };

  return (
    <div className="space-y-24 py-12">
      {/* Hero Section */}
      <section className="text-center max-w-4xl mx-auto px-4 sm:px-6 space-y-6">
        <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-brand-50 border border-brand-200 text-brand-900 text-xs font-mono font-medium">
          <Sparkles className="w-3.5 h-3.5 text-brand-600" />
          Evidence-Based Builder Intelligence
        </div>

        <h1 className="text-4xl sm:text-6xl font-extrabold text-stone-950 tracking-tight leading-[1.15]">
          Learn from the journey,{' '}
          <span className="italic font-serif font-normal text-stone-600">
            not just the outcome.
          </span>
        </h1>

        <p className="text-lg sm:text-xl text-stone-600 max-w-2xl mx-auto font-normal leading-relaxed">
          Transform publicly authorized social media data into structured, chronological journey reports. Discover how founders test ideas, survive pivots, launch products, and build in public.
        </p>

        <div className="flex flex-col sm:flex-row items-center justify-center gap-3 pt-4">
          <Link
            to="/discover"
            className="w-full sm:w-auto px-6 py-3 rounded-lg bg-stone-950 text-stone-50 font-medium hover:bg-stone-800 transition-all shadow-sm flex items-center justify-center gap-2"
          >
            <Compass className="w-4 h-4" />
            Explore Builder Directory
          </Link>
          <button
            onClick={handleDemoClick}
            className="w-full sm:w-auto px-6 py-3 rounded-lg bg-white border border-stone-200 text-stone-800 font-medium hover:bg-stone-50 transition-all flex items-center justify-center gap-2"
          >
            <Sparkles className="w-4 h-4 text-brand-600" />
            Instant 1-Click Interactive Demo
          </button>
        </div>

        {/* Value Pillars */}
        <div className="pt-8 flex flex-wrap items-center justify-center gap-6 text-xs text-stone-500 font-mono">
          <span className="flex items-center gap-1.5">
            <CheckCircle2 className="w-4 h-4 text-emerald-600" />
            Verifiable Post Evidence
          </span>
          <span className="flex items-center gap-1.5">
            <CheckCircle2 className="w-4 h-4 text-emerald-600" />
            Explicit vs Inferred Tagging
          </span>
          <span className="flex items-center gap-1.5">
            <CheckCircle2 className="w-4 h-4 text-emerald-600" />
            No Scraping or Fabricated Metrics
          </span>
        </div>
      </section>

      {/* Interactive Teaser Timeline */}
      <section className="max-w-5xl mx-auto px-4 sm:px-6">
        <div className="bg-white rounded-2xl border border-stone-200 p-8 shadow-sm space-y-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between border-b border-stone-100 pb-4 gap-2">
            <div>
              <span className="text-xs font-mono uppercase tracking-wider text-brand-700 font-semibold">
                Interactive Journey Anatomy
              </span>
              <h2 className="text-xl font-bold text-stone-950">How an Idea Becomes a Business</h2>
            </div>
            <Link
              to="/discover"
              className="text-xs font-medium text-stone-600 hover:text-stone-950 flex items-center gap-1"
            >
              View Full Interactive Report <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 relative">
            <div className="p-5 rounded-xl border border-stone-200 bg-stone-50/70 space-y-3 relative">
              <div className="flex items-center justify-between text-xs text-stone-500">
                <span className="font-mono text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded font-medium border border-emerald-200">
                  Day 1 • Idea
                </span>
                <span className="font-mono text-[11px]">Explicit Fact</span>
              </div>
              <h3 className="font-semibold text-stone-900 text-sm">Problem Formulation</h3>
              <p className="text-stone-600 text-xs leading-relaxed">
                "What if I built a simple city ranking for digital nomads based on cost and wifi?"
              </p>
              <div className="text-[11px] text-stone-400 font-mono pt-2 border-t border-stone-200">
                Evidence: Direct post statement
              </div>
            </div>

            <div className="p-5 rounded-xl border border-stone-200 bg-stone-50/70 space-y-3 relative">
              <div className="flex items-center justify-between text-xs text-stone-500">
                <span className="font-mono text-indigo-700 bg-indigo-50 px-2 py-0.5 rounded font-medium border border-indigo-200">
                  Week 4 • Prototype
                </span>
                <span className="font-mono text-[11px]">Direct Video</span>
              </div>
              <h3 className="font-semibold text-stone-900 text-sm">First Functional Demo</h3>
              <p className="text-stone-600 text-xs leading-relaxed">
                60-second screen capture demonstrating search filters. Discussion depth surges +310%.
              </p>
              <div className="text-[11px] text-stone-400 font-mono pt-2 border-t border-stone-200">
                Evidence: Media clip + reply surge
              </div>
            </div>

            <div className="p-5 rounded-xl border border-stone-200 bg-stone-50/70 space-y-3 relative">
              <div className="flex items-center justify-between text-xs text-stone-500">
                <span className="font-mono text-amber-700 bg-amber-50 px-2 py-0.5 rounded font-medium border border-amber-200">
                  Month 3 • Customer
                </span>
                <span className="font-mono text-[11px]">High Confidence</span>
              </div>
              <h3 className="font-semibold text-stone-900 text-sm">First Paying Customer</h3>
              <p className="text-stone-600 text-xs leading-relaxed">
                "First $5 subscription recorded! Real validation from a stranger online."
              </p>
              <div className="text-[11px] text-stone-400 font-mono pt-2 border-t border-stone-200">
                Evidence: Verifiable milestone post
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Featured Builders Teaser */}
      <section className="max-w-5xl mx-auto px-4 sm:px-6 space-y-8">
        <div className="text-center space-y-2">
          <h2 className="text-2xl font-bold text-stone-950">Curated Builder Studies</h2>
          <p className="text-stone-600 text-sm">
            Empirical timelines reconstructed from public posts and timestamps.
          </p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {[
            { name: 'Pieter Levels', handle: '@levelsio', category: 'SaaS', status: 'Nomad List & Photo AI', followers: '542k' },
            { name: 'Marc Lou', handle: '@marc_louvion', category: 'Micro-SaaS', status: 'ShipFast & ByeByeBye', followers: '148k' },
            { name: 'Danny Postma', handle: '@dannypostmaa', category: 'AI', status: 'HeadshotPro & Landingfolio', followers: '172k' },
            { name: 'Tony Dinh', handle: '@tdinh_l', category: 'Developer Tools', status: 'TypingMind & DevUtils', followers: '121k' }
          ].map((b, i) => (
            <Link
              key={i}
              to="/discover"
              className="bg-white p-5 rounded-xl border border-stone-200 hover:border-stone-400 transition-all space-y-3 group"
            >
              <div className="flex items-center justify-between">
                <span className="text-[11px] font-mono px-2 py-0.5 rounded bg-stone-100 text-stone-700">
                  {b.category}
                </span>
                <span className="text-[11px] font-mono text-stone-400">{b.followers} followers</span>
              </div>
              <div>
                <h4 className="font-semibold text-stone-900 group-hover:text-brand-700 transition-colors">{b.name}</h4>
                <span className="text-xs text-stone-500 font-mono">{b.handle}</span>
              </div>
              <p className="text-xs text-stone-600 line-clamp-2">{b.status}</p>
              <div className="text-xs text-brand-700 font-medium flex items-center gap-1 pt-2 border-t border-stone-100">
                Inspect Journey <ArrowRight className="w-3 h-3" />
              </div>
            </Link>
          ))}
        </div>
      </section>

      {/* Core Differentiation Callout */}
      <section className="max-w-4xl mx-auto px-4 sm:px-6">
        <div className="bg-stone-900 text-stone-50 rounded-2xl p-8 sm:p-12 space-y-6">
          <span className="text-xs font-mono uppercase tracking-wider text-brand-400 font-semibold">
            Product Manifesto
          </span>
          <h2 className="text-2xl sm:text-3xl font-bold tracking-tight">
            Built for trustworthy insight, not impressive-looking hallucinations.
          </h2>
          <p className="text-stone-300 text-sm sm:text-base leading-relaxed">
            Most social analytics tools just count vanity impressions. Builder Journey connects public timestamps, media attachments, and stated milestones into a chronological narrative. We disclose data limitations, label inferences, and never invent private metrics.
          </p>
          <div className="pt-2">
            <Link
              to="/action-plan"
              className="inline-flex items-center gap-2 px-5 py-2.5 rounded-lg bg-brand-500 text-stone-950 font-semibold hover:bg-brand-400 transition-colors text-sm"
            >
              Generate Your Personalized Action Plan <ArrowRight className="w-4 h-4" />
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
};

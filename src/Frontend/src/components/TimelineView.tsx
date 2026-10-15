import React, { useState } from 'react';
import {
  Rocket,
  Lightbulb,
  Video,
  DollarSign,
  RefreshCw,
  GitFork,
  Award,
  TrendingUp,
  AlertOctagon,
  HelpCircle,
  ExternalLink,
  ShieldCheck,
  ShieldAlert,
  Search,
  Filter
} from 'lucide-react';
import { JourneyEvent, JourneyEventCategory } from '../types';
import { EvidenceDrawer } from './EvidenceDrawer';

interface TimelineViewProps {
  events: JourneyEvent[];
  limitations?: string[];
}

export const TimelineView: React.FC<TimelineViewProps> = ({ events, limitations }) => {
  const [selectedCategory, setSelectedCategory] = useState<string>('All');
  const [selectedEvent, setSelectedEvent] = useState<JourneyEvent | null>(null);
  const [searchQuery, setSearchQuery] = useState('');

  const getCategoryIcon = (cat: JourneyEventCategory) => {
    switch (cat) {
      case 'PublicLaunch': return <Rocket className="w-4 h-4 text-emerald-600" />;
      case 'FirstCustomer': return <DollarSign className="w-4 h-4 text-amber-600" />;
      case 'InitialIdea': return <Lightbulb className="w-4 h-4 text-indigo-600" />;
      case 'PrototypeDemo': return <Video className="w-4 h-4 text-purple-600" />;
      case 'Pivot': return <GitFork className="w-4 h-4 text-rose-600" />;
      case 'FailureOrLesson': return <AlertOctagon className="w-4 h-4 text-orange-600" />;
      case 'ProductIteration': return <RefreshCw className="w-4 h-4 text-blue-600" />;
      case 'AudienceMilestone': return <Award className="w-4 h-4 text-yellow-600" />;
      case 'Growth': return <TrendingUp className="w-4 h-4 text-teal-600" />;
      default: return <HelpCircle className="w-4 h-4 text-stone-500" />;
    }
  };

  const filteredEvents = events.filter(e => {
    const matchesCategory = selectedCategory === 'All' || e.category === selectedCategory;
    const matchesSearch =
      searchQuery === '' ||
      e.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      e.description.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesCategory && matchesSearch;
  });

  return (
    <div className="space-y-6">
      {/* Category Filter Pills & Search */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 bg-white p-3 rounded-xl border border-stone-200 shadow-sm">
        <div className="flex items-center gap-1.5 overflow-x-auto max-w-full pb-1 sm:pb-0">
          {['All', 'PublicLaunch', 'FirstCustomer', 'PrototypeDemo', 'InitialIdea', 'Pivot', 'FailureOrLesson'].map((cat) => (
            <button
              key={cat}
              onClick={() => setSelectedCategory(cat)}
              className={`px-3 py-1 rounded-lg text-xs font-medium whitespace-nowrap transition-colors ${
                selectedCategory === cat
                  ? 'bg-stone-900 text-stone-50 shadow-sm'
                  : 'bg-stone-100 text-stone-600 hover:bg-stone-200'
              }`}
            >
              {cat === 'All' ? 'All Milestones' : cat.replace(/([A-Z])/g, ' $1').trim()}
            </button>
          ))}
        </div>

        <div className="relative w-full sm:w-64">
          <Search className="w-3.5 h-3.5 text-stone-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder="Search events..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full text-xs pl-8 pr-3 py-1.5 rounded-lg border border-stone-200 focus:outline-none focus:ring-1 focus:ring-stone-400 bg-stone-50"
          />
        </div>
      </div>

      {/* Timeline nodes */}
      {filteredEvents.length === 0 ? (
        <div className="text-center py-12 bg-white rounded-xl border border-stone-200 text-stone-500 text-sm">
          No milestones match the selected filter.
        </div>
      ) : (
        <div className="relative border-l-2 border-stone-200 ml-4 sm:ml-8 pl-6 sm:pl-8 space-y-8 my-8">
          {filteredEvents.map((ev) => (
            <div key={ev.id} className="relative group">
              {/* Timeline marker icon */}
              <div className="absolute -left-[35px] sm:-left-[43px] top-1.5 w-8 h-8 rounded-full bg-white border-2 border-stone-300 flex items-center justify-center shadow-xs group-hover:border-stone-600 transition-colors">
                {getCategoryIcon(ev.category)}
              </div>

              {/* Event Card */}
              <div className="bg-white rounded-xl border border-stone-200 p-5 shadow-xs hover:shadow-md transition-all space-y-3">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
                  <div className="flex items-center gap-2">
                    <span className="font-mono text-xs text-stone-500 font-semibold">
                      {new Date(ev.eventDate).toLocaleDateString(undefined, {
                        month: 'short',
                        day: 'numeric',
                        year: 'numeric',
                      })}
                    </span>
                    <span className="text-stone-300">•</span>
                    <span className="text-xs px-2 py-0.5 rounded bg-stone-100 text-stone-700 font-medium">
                      {ev.categoryName || ev.category}
                    </span>
                  </div>

                  <div className="flex items-center gap-2">
                    {ev.isInferred ? (
                      <span className="text-[10px] font-mono px-2 py-0.5 rounded bg-amber-50 text-amber-900 border border-amber-200 flex items-center gap-1">
                        <ShieldAlert className="w-3 h-3 text-amber-600" />
                        Inferred
                      </span>
                    ) : (
                      <span className="text-[10px] font-mono px-2 py-0.5 rounded bg-emerald-50 text-emerald-900 border border-emerald-200 flex items-center gap-1">
                        <ShieldCheck className="w-3 h-3 text-emerald-600" />
                        Explicit Fact
                      </span>
                    )}

                    <span
                      className={`text-[10px] font-mono px-1.5 py-0.5 rounded ${
                        ev.confidence === 'High'
                          ? 'bg-stone-100 text-stone-800 font-medium'
                          : 'bg-stone-50 text-stone-500'
                      }`}
                    >
                      {ev.confidence} Conf.
                    </span>
                  </div>
                </div>

                <h4 className="text-base font-semibold text-stone-950 group-hover:text-brand-900 transition-colors">
                  {ev.title}
                </h4>

                <p className="text-stone-700 text-sm leading-relaxed">
                  {ev.description}
                </p>

                {/* Evidence trigger */}
                <div className="pt-2 flex items-center justify-between border-t border-stone-100 text-xs">
                  <button
                    onClick={() => setSelectedEvent(ev)}
                    className="inline-flex items-center gap-1.5 text-stone-900 font-medium hover:text-brand-700 bg-stone-100 hover:bg-stone-200 px-3 py-1.5 rounded-lg transition-colors cursor-pointer"
                  >
                    <span>Inspect Evidence</span>
                    <span className="font-mono bg-white px-1.5 py-0.2 rounded text-[10px] text-stone-600 border border-stone-200">
                      {ev.evidences?.length || 0} posts
                    </span>
                  </button>

                  {ev.evidences?.[0]?.postUrl && (
                    <a
                      href={ev.evidences[0].postUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-stone-400 hover:text-stone-700 flex items-center gap-1"
                    >
                      Original Source
                      <ExternalLink className="w-3 h-3" />
                    </a>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Limitations note box */}
      {limitations && limitations.length > 0 && (
        <div className="bg-stone-100 border border-stone-200 rounded-xl p-4 text-xs text-stone-600 space-y-2">
          <div className="font-semibold text-stone-800 flex items-center gap-1.5">
            <HelpCircle className="w-3.5 h-3.5" />
            Timeline Reconstruction Limitations
          </div>
          <ul className="list-disc list-inside space-y-1 text-stone-600 pl-1">
            {limitations.map((lim, i) => (
              <li key={i}>{lim}</li>
            ))}
          </ul>
        </div>
      )}

      {/* Evidence Drawer modal */}
      {selectedEvent && (
        <EvidenceDrawer
          isOpen={!!selectedEvent}
          onClose={() => setSelectedEvent(null)}
          title={selectedEvent.title}
          evidences={selectedEvent.evidences}
          limitationsNote={selectedEvent.limitationsNote}
          isInferred={selectedEvent.isInferred}
        />
      )}
    </div>
  );
};

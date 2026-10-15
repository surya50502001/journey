import React from 'react';
import { ShieldCheck, ExternalLink } from 'lucide-react';
import { Link } from 'react-router-dom';

export const Footer: React.FC = () => {
  return (
    <footer className="border-t border-stone-200 bg-stone-100/60 mt-20 text-stone-600 text-xs py-10">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 space-y-6">
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <div className="font-semibold text-stone-900 text-sm">Builder Journey Intelligence Platform</div>
            <div className="italic text-stone-500 font-serif text-sm mt-0.5">
              "Learn from the journey, not just the outcome."
            </div>
          </div>
          <div className="flex items-center gap-4">
            <Link to="/settings" className="hover:text-stone-950 transition-colors">API Transparency & Policy</Link>
            <Link to="/discover" className="hover:text-stone-950 transition-colors">Curated Directory</Link>
            <Link to="/compare" className="hover:text-stone-950 transition-colors">Comparator</Link>
          </div>
        </div>

        <div className="border-t border-stone-200/80 pt-4 flex flex-col md:flex-row justify-between items-start md:items-center gap-3 text-[11px] text-stone-500">
          <div className="flex items-center gap-1.5">
            <ShieldCheck className="w-3.5 h-3.5 text-stone-700" />
            <span>
              Strict API compliance: unauthorized scraping is prohibited. Data snapshots reflect public author statements and authorized platform endpoints.
            </span>
          </div>
          <div>
            © 2026 Builder Journey Platform. Grounded in verifiable evidence.
          </div>
        </div>
      </div>
    </footer>
  );
};

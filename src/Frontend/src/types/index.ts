export type PlatformType = 'Curated' | 'X' | 'YouTube' | 'Instagram' | 'TikTok' | 'LinkedIn';

export type JourneyEventCategory =
  | 'InitialIdea'
  | 'ProblemValidation'
  | 'PrototypeDemo'
  | 'PublicLaunch'
  | 'FirstCustomer'
  | 'ProductIteration'
  | 'Pivot'
  | 'AudienceMilestone'
  | 'BusinessMilestone'
  | 'FailureOrLesson'
  | 'Growth';

export type ConfidenceLevel = 'Low' | 'Medium' | 'High';

export type DatePrecision = 'Exact' | 'Month' | 'Year' | 'Approximate';

export type EvidenceType =
  | 'DirectStatement'
  | 'MetricSurge'
  | 'TemporalContext'
  | 'MediaDemo'
  | 'PublicLink';

export type ContentCategory =
  | 'BuildInPublic'
  | 'ProductDemo'
  | 'TechnicalTutorial'
  | 'EducationalThread'
  | 'PersonalStory'
  | 'LaunchAnnouncement'
  | 'CustomerFeedback'
  | 'LessonsLearned'
  | 'DiscussionQuestion'
  | 'Promotional';

export type UserStage = 'Idea' | 'Prototype' | 'Launched' | 'FirstUsers' | 'Growth';

export type UserObjective = 'BuildProduct' | 'AcquireUsers' | 'ImproveContent' | 'GrowAudience';

export type VisibilityType = 'Private' | 'Unlisted' | 'Public';

export type SyncStatus = 'Queued' | 'Running' | 'Completed' | 'Failed' | 'RateLimited';

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
  currentStage: UserStage;
  primaryObjective: UserObjective;
}

export interface UserProfile {
  id: string;
  email: string;
  fullName: string;
  role: string;
  currentStage: UserStage;
  primaryObjective: UserObjective;
  preferredPlatform: PlatformType;
  hoursAvailablePerWeek: number;
  skillsAndConstraints: string;
  createdAt: string;
}

export interface BuilderCategory {
  id: string;
  name: string;
  slug: string;
  description: string;
  displayOrder: number;
  builderCount: number;
}

export interface BuilderProfileSummary {
  id: string;
  handle: string;
  name: string;
  bio: string;
  avatarUrl: string;
  profileUrl: string;
  platform: PlatformType;
  categoryName: string;
  categorySlug: string;
  currentStatus: string;
  isCurated: boolean;
  isVerified: boolean;
  followersCount: number;
  totalPostsCount: number;
  documentedMilestonesCount: number;
  documentedStartDate: string | null;
  lastSyncedAt: string | null;
}

export interface BuilderProfileDetail extends BuilderProfileSummary {
  followingCount: number;
  totalMilestones: number;
  totalInsights: number;
  isInUserWatchlist: boolean;
}

export interface JourneyEventEvidence {
  id: string;
  contentPostId: string;
  postContent: string;
  publishedAt: string;
  postUrl: string;
  postType: string;
  likes: number;
  replies: number;
  reposts: number;
  evidenceType: EvidenceType;
  excerpt: string;
  weight: number;
}

export interface JourneyEvent {
  id: string;
  title: string;
  description: string;
  eventDate: string;
  precision: DatePrecision;
  category: JourneyEventCategory;
  categoryName: string;
  confidence: ConfidenceLevel;
  isInferred: boolean;
  limitationsNote: string;
  evidences: JourneyEventEvidence[];
}

export interface JourneyTimeline {
  builderId: string;
  builderName: string;
  builderHandle: string;
  avatarUrl: string;
  documentedStartDate: string | null;
  currentStatus: string;
  totalEvents: number;
  highConfidenceEventsCount: number;
  inferredEventsCount: number;
  events: JourneyEvent[];
  reconstructionLimitations: string[];
}

export interface CategoryDistribution {
  category: ContentCategory;
  categoryName: string;
  postCount: number;
  percentage: number;
  averageLikes: number;
  averageReplies: number;
  averageReposts: number;
}

export interface PostingCadence {
  periodLabel: string;
  postCount: number;
  averageLikes: number;
  averageReplies: number;
}

export interface FormatPerformance {
  formatName: string;
  count: number;
  averageLikes: number;
  averageReplies: number;
  averageReposts: number;
  engagementMultiplier: number;
}

export interface TopTopic {
  topic: string;
  frequency: number;
  averageEngagement: number;
  samplePostSnippets: string[];
}

export interface PostSummary {
  id: string;
  platformPostId: string;
  content: string;
  publishedAt: string;
  postUrl: string;
  postType: string;
  classifiedCategory: ContentCategory | null;
  likes: number;
  replies: number;
  reposts: number;
  views: number;
  bookmarks: number;
}

export interface ContentStrategySummary {
  builderId: string;
  builderName: string;
  builderHandle: string;
  totalAnalyzedPosts: number;
  earliestAnalyzedPost: string | null;
  latestAnalyzedPost: string | null;
  averagePostsPerWeek: number;
  consistencyScore: string;
  categories: CategoryDistribution[];
  cadenceHistory: PostingCadence[];
  formatPerformances: FormatPerformance[];
  topTopics: TopTopic[];
  topResonancePosts: PostSummary[];
  strategyOverview: string;
  limitationsNotice: string;
}

export interface BreakthroughEvidence {
  postId: string;
  content: string;
  publishedAt: string;
  postUrl: string;
  likes: number;
  replies: number;
  roleInShift: string;
}

export interface Breakthrough {
  id: string;
  title: string;
  summary: string;
  metricObserved: string;
  periodStart: string;
  periodEnd: string;
  magnitudeChange: string;
  plausibleExplanations: string;
  limitationsAndAlternatives: string;
  evidences: BreakthroughEvidence[];
}

export interface BuilderComparisonMetric {
  metricName: string;
  valuesByBuilderId: Record<string, string>;
  contextNote: string;
}

export interface BuilderComparisonProfile {
  id: string;
  name: string;
  handle: string;
  avatarUrl: string;
  categoryName: string;
  followersCount: number;
  totalPostsCount: number;
  documentedMilestonesCount: number;
  documentedStartDate: string | null;
  primaryContentStyle: string;
  launchPacing: string;
}

export interface BuilderComparisonResult {
  builders: BuilderComparisonProfile[];
  comparativeMetrics: BuilderComparisonMetric[];
  categoryMixByBuilder: Record<string, CategoryDistribution[]>;
  alignedMilestonesByBuilder: Record<string, JourneyEvent[]>;
  comparativeSynthesis: string;
  analyticalLimitationsNotice: string;
}

export interface ActionPlanTask {
  id: string;
  weekNumber: number;
  focusArea: string;
  title: string;
  actionDescription: string;
  suggestedContentFormat: string;
  milestoneTarget: string;
  relevantBuilderProfileId?: string;
  relevantBuilderName?: string;
  relevantBuilderHandle?: string;
  relevantEventId?: string;
  relevantEventTitle?: string;
  supportingRationale: string;
  successMetric: string;
  isCompleted: boolean;
}

export interface ActionPlan {
  id: string;
  userGoalId: string;
  stage: UserStage;
  objective: UserObjective;
  hoursPerWeek: number;
  targetPlatform: PlatformType;
  title: string;
  summary: string;
  methodologyExplanation: string;
  generatedAt: string;
  tasks: ActionPlanTask[];
}

export interface WatchlistEntry {
  id: string;
  builderProfileId: string;
  builderName: string;
  builderHandle: string;
  avatarUrl: string;
  categoryName: string;
  followersCount: number;
  currentStatus: string;
  addedAt: string;
  notes: string;
  latestMilestoneDate: string | null;
  latestMilestoneTitle: string | null;
}

export interface LessonLearned {
  principle: string;
  builderContext: string;
  practicalApplication: string;
  supportingEvidence: string;
}

export interface JourneyReport {
  id: string;
  builderProfileId: string;
  builderName: string;
  builderHandle: string;
  avatarUrl: string;
  bio: string;
  categoryName: string;
  currentStatus: string;
  documentedStartDate: string | null;
  title: string;
  executiveSummary: string;
  contentDnaSummary: string;
  keyLessons: LessonLearned[];
  limitationsDisclosed: string;
  generatedAt: string;
  version: string;
  shareSlug: string | null;
  visibility: VisibilityType;
  timeline: JourneyTimeline;
  contentStrategy: ContentStrategySummary;
  breakthroughs: Breakthrough[];
}

export interface PlatformCapability {
  platform: PlatformType;
  supportsLikes: boolean;
  supportsReplies: boolean;
  supportsReposts: boolean;
  supportsViews: boolean;
  supportsBookmarks: boolean;
  supportsHistoricalFollowers: boolean;
  supportsFullHistory: boolean;
  rateLimitDescription: string;
  dataRetentionPolicy: string;
  attributionRequirement: string;
  limitations: string[];
}

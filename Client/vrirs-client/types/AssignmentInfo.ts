import type { AssignmentCategory } from "./AssignmentCategory";
import type { AssignmentLocation } from "./AssignmentLocation";
import type { AssignmentStatus } from "./AssignmentStatus";

export type AssignmentInfo = {
  id: string;

  courseId: string;
  courseName: string;

  createdByUserId: string;
  createdByUserFullName: string;

  submissionTestId: string | null;

  title: string;
  description: string | null;

  location: AssignmentLocation;
  category: AssignmentCategory;

  opensAt: string | null;
  dueAt: string | null;

  status: AssignmentStatus;

  allowProjectUpload: boolean;
  allowMultipleAttempts: boolean;

  maxPoints: number;
  minPoints: number;
};
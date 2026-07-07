import type {SubmissionStatus} from "./SubmissionStatus"

export interface SubmissionInfo {
  id: string;
  assignmentId: string;
  studentUserId: string;

  assignmentTitle: string;
  studentFullName: string;
  studentIndex?: string;

  title: string;
  description?: string;

  status: SubmissionStatus;

  submittedAt?: string; 
}
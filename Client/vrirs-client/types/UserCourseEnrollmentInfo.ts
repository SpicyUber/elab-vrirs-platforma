import type { EnrollmentRole } from "./EnrollmentRole";
import type { EnrollmentStatus } from "./EnrollmentStatus";

export interface UserCourseEnrollmentInfo {
    userId: string;
    courseId: string;
    userAvatarInBase64: string;
    userFullName: string;
    courseName: string;
    userEmail: string;
    enrollmentStatus: EnrollmentRole;
    enrollmentRole: EnrollmentStatus;
}
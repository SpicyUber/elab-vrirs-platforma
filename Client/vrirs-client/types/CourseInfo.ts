import type { CourseCategory } from "./CourseCategory";

export interface CourseInfo {
    id: string; // Guid
    name: string;
    description?: string | null;
    createdByUserId: string; // Guid
    createdByUserFullName: string;
    startDate: string; // DateTime from API JSON
    endDate: string;   // DateTime from API JSON
    category: CourseCategory;
}
import type { UserProfileInfo } from "./UserProfileInfo";

export type UserSearchResultPage = {
  userProfiles: UserProfileInfo[];
  entiresPerPage: number;
  pageNumer: number;
  maxPages: number;
};
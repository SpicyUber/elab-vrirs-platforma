import { useEffect, useState } from "react";
import { Box, Card, CardContent, Chip, Stack, Typography, CircularProgress, Button, Divider } from "@mui/material";
import Sidebar from "../components/Sidebar";
import { useUser } from "../hooks/useUser";
import {useServer} from "../hooks/useServer";
import type { CourseInfo } from "../types/CourseInfo";
import type { AssignmentInfo } from "../types/AssignmentInfo";
import { Navigate, useNavigate, useParams } from "react-router-dom";


const Course = () => {
  const { courseId } = useParams();
  const { user } = useUser();
  const {baseUrl} = useServer(); 
  const [assignments, setAssignments] = useState<AssignmentInfo[]>([]);
  const [course, setCourse] = useState<CourseInfo>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [role, setRole] = useState<string>("");
  const navigate = useNavigate();

  useEffect(() => {
    async function fetchCourseAssignments() {

      setLoading(true);
    setAssignments([]);
    setError(null);
      if(user?.roles.toLowerCase().includes("admin"))return;
      
      try {
        const roleRequest =  await fetch(baseUrl + "/api/courses/" + courseId + "/my-enrollment-role", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + user?.token,
          },
        });

        if (!roleRequest.ok && !user?.roles.toLocaleLowerCase().includes("admin")) {
          setError("You are not enrolled in this course.");
          return;
        }
        let roleResult = await roleRequest.json();
        setRole(roleResult);
        let route: string = (roleResult.toLowerCase() === "student")? 
        `from-course/${courseId}`
        : `from-course/${courseId}/mine`

        if(user?.roles.toLowerCase().includes("admin"))
          route = `from-course/${courseId}/all`

        const res = await fetch(baseUrl + "/api/assignments/"+route, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + user?.token,
          },
        });

        if (!res.ok) {
          setError("Could not load course assignments."+ await res.text());
          return;
        }

        const data: AssignmentInfo[] = await res.json();
        setAssignments(data);
      } catch (e) {
        setError("Something went wrong while loading course assignments.");
      } finally {
        setLoading(false);
      }
    }

    fetchCourseAssignments();
  }, [user?.token, courseId]);

  useEffect(() => {
    async function fetchCourse() {
      try {
        const res = await fetch(baseUrl + "/api/courses/"+courseId, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + user?.token,
          },
        });

        if (!res.ok) {
          setError("Could not load course.");
          return;
        }

        const data: CourseInfo = await res.json();
        setCourse(data);
      } catch (e) {
        setError("Something went wrong while loading course.");
      } finally {
        setLoading(false);
      }
    }

    fetchCourse();
  }, [user?.token, courseId]);

 async function create() {
    setLoading(true);
  try {
    const res = await fetch(
      `${baseUrl}/api/assignments/from-course/${courseId}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + user?.token,
        },
      }
    );

    if (!res.ok) {
      throw new Error("Failed to create assignment");
    }

    const data: AssignmentInfo = await res.json();

    navigate(`/assignments/${data.id}/edit`);
  } catch (e) {
    console.error(e);
  }
}

const archiveCourse = async () => {
  try {
    const res = await fetch(`${baseUrl}/api/courses/${courseId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${user?.token}`,
      },
    });

    if (!res.ok) {
      alert("Failed to archive course.");
      return;
    }

    navigate("/my-courses"); 
  } catch {
    alert("Error archiving course.");
  }
};

  return (
    <Box>
      <Sidebar />
      <Box>
        <Typography variant="h2">
          {course?.name}
        </Typography>
        <Typography variant="h4">
          {course?.description}
        </Typography>
        <br></br>
        {!user?.roles.toLowerCase().includes("admin") &&<Typography variant="h5">
          Course Assignments
        </Typography>}

        {loading && <CircularProgress />}

        {!loading && error && (
          <Typography color="error">{error}</Typography>
        )}


        {!loading && !error && assignments.length > 0 && (
          <Stack spacing={{ xs: 1, sm: 2 }}
  direction="row"
  useFlexGap
  sx={{ flexWrap: 'wrap', paddingLeft: 10, paddingRight: 10 }}>
            {assignments.map((assignments) => (
              <Button onClick={()=>navigate(`/assignments/${assignments.id}`)}>
              <Card key={assignments.id} sx={{ width: 280 }}>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {assignments.title}
                  </Typography>

                  <Chip
                    label={assignments.category}
                    color={
                      assignments.category === "Csmp"
                        ? "error"
                        : assignments.category === "Gpss"
                        ? "warning"
                        : "default"
                    }
                    size="small"
                    sx={{ mb: 1 }}
                  />
                  <Chip
                    label={assignments.location}
                    color={
                      assignments.location === "Class"
                        ? "default"
                        : "warning"
                    }
                    size="small"
                    sx={{ mb: 1 }}
                  />
                  <Typography variant="body2" color="text.secondary">
                    Points: {assignments.minPoints} - {assignments.maxPoints}
                  </Typography>

                  <Typography variant="body2" color="text.secondary">
                    Created by: {assignments.createdByUserFullName}
                  </Typography>

                  <Typography variant="caption" color="text.secondary">
                    Due at: {assignments.dueAt == null? "---" :new Date(assignments.dueAt + "Z").toLocaleDateString()}
                  </Typography>
                </CardContent>
              </Card>
              </Button>
            ))}
          </Stack>
        )}
      </Box>
      <Divider></Divider>
      {user?.roles.toLowerCase().includes("admin")   && <>
        <Button
      variant="contained"
      onClick={() => navigate(`/courses/${courseId}/edit`)} sx={{ margin: 2 }}>
        Edit Course
      </Button><Button onClick={archiveCourse}>Archive Course</Button>  </>}
  {user?.roles.toLowerCase().includes("teacher") && <> <Button
    variant="contained"
    color="primary"
    onClick={create}
    sx={{ margin: 2 }}
  >
    Add Assignment
  </Button></>}
 <Button
    variant="contained"
    color="primary"
    onClick={()=>navigate(`/courses/${courseId}/members`)}
    sx={{ margin: 2 }}
  >
    Members
  </Button>

    </Box>
    
  );
};

export default Course;
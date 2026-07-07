import { useEffect, useState } from "react";
import {
  Box,
  Card,
  CardContent,
  CircularProgress,
  Stack,
  Typography,
   Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  MenuItem
} from "@mui/material";
import Sidebar from "../components/Sidebar";
import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
import type { CourseInfo } from "../types/CourseInfo";
import { useNavigate } from "react-router-dom";
import type { CourseCategory } from "../types/CourseCategory";

const MyCourses = () => {
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [courses, setCourses] = useState<CourseInfo[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
 
  const [isAdmin, setIsAdmin] = useState<boolean>(false);
  
  const [open, setOpen] = useState(false);

  const [form, setForm] = useState<{
  name: string;
  description: string;
  category: CourseCategory;
  }>({
    name: "",
    description: "",
    category: "I",
  });

  const categories: CourseCategory[] = ["I", "II", "III", "IV", "MASTER"];

  const now = () : Date => {let d = new Date(); d.setMonth(10); d.setDate(1); return d; }
  const future= () : Date => {let d = new Date(); d.setMonth(10); d.setDate(1); d.setFullYear(d.getFullYear()+1); return d; }
const handleCreateCourse = async () => {
  if (!user) return;

  try {
    const res = await fetch(`${baseUrl}/api/courses/create`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + user.token,
      },
      body: JSON.stringify({
        name: form.name,
        description: form.description,
        startDate: now().toISOString(),
        endDate: future().toISOString(),
        category: form.category,
      }),
    });

    if (!res.ok) {
      setError("Failed to create course.");
      return;
    }

    const created: CourseInfo = await res.json();

    setOpen(false);
    navigate(`/courses/${created.id}/edit`);
  } catch {
    setError("Something went wrong.");
  }
};
  useEffect(() => {
    async function fetchCourses() {
      let role = "Student";
      
      if (user?.roles?.toLowerCase().includes("teacher"))
        role = "Teacher";

      if (user?.roles?.toLowerCase().includes("admin"))
        role = "Admin";
      
      setIsAdmin(role === "Admin")

      let route: string = `/api/courses/mine?role=${role}`;
console.log(user?.roles + "|"+role);
      if(role === "Admin")
        route = `/api/courses/all`;

      try {
        const res = await fetch(
          `${baseUrl}${route}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: "Bearer " + user?.token,
            },
          }
        );

        if (!res.ok) {
          setError("Could not load courses.");
          return;
        }

        const data: CourseInfo[] = await res.json();
        setCourses(data);
      } catch {
        setError("Something went wrong while loading courses.");
      } finally {
        setLoading(false);
      }
    }

    if (user?.token) {
      fetchCourses();
    }
  }, [baseUrl, user?.token]);

  return (
    <Box>
      <Sidebar />

      <Box>
        <Box>
          <Typography variant="h5">
            Course View
          </Typography>
        </Box>

        {loading && <CircularProgress />}

        {!loading && error && (
          <Typography color="error">{error}</Typography>
        )}

        {!loading && !error && courses.length === 0 && (
          <Typography color="text.secondary" >
            No courses found.
          </Typography>
        )}

        {!loading && !error && courses.length > 0 && (
          <Stack
            spacing={2}
            direction="row"
            useFlexGap
            sx={{
              flexWrap: "wrap",
              px: 10,
            }}
          >
            {courses.map((course) => (
              <Card
                key={course.id}
                sx={{ width: 280, cursor: "pointer" }}
                onClick={() => navigate(`/courses/${course.id}`)}
              >
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {course.name}
                  </Typography>

                  <Typography variant="body2" color="text.secondary">
                    {course.description}
                  </Typography>
                </CardContent>
              </Card>
            ))}
          </Stack>
        )}
        
      </Box>
      {isAdmin && <><Dialog open={open} onClose={() => setOpen(false)} fullWidth>
  <DialogTitle>Create Course</DialogTitle>

  <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
    <TextField
      label="Name"
      value={form.name}
      onChange={(e) => setForm({ ...form, name: e.target.value })}
      fullWidth
    />

    <TextField
      label="Description"
      value={form.description}
      onChange={(e) => setForm({ ...form, description: e.target.value })}
      fullWidth
      multiline
      rows={3}
    />

    <TextField
      select
      label="Category"
      value={form.category}
      onChange={(e) =>
        setForm({ ...form, category: e.target.value as CourseCategory })
      }
    >
      {categories.map((c) => (
        <MenuItem key={c} value={c}>
          {c}
        </MenuItem>
      ))}
    </TextField>
  </DialogContent>

  <DialogActions>
    <Button onClick={() => setOpen(false)}>Cancel</Button>
    <Button
      variant="contained"
      onClick={handleCreateCourse}
      disabled={!form.name}
    >
      Create
    </Button>
  </DialogActions>
</Dialog>
<Button variant="contained" onClick={() => setOpen(true)}>
  Add Course
</Button></>}
    </Box>
  );
};

export default MyCourses;
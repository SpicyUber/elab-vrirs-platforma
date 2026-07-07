import { Drawer, List, ListItemButton, ListItemText, Typography, IconButton  } from '@mui/material';
import MenuRoundedIcon from "@mui/icons-material/MenuRounded";
import MenuOpenIcon from "@mui/icons-material/MenuOpen";
import { useEffect, useState } from 'react';
import { useServer } from '../hooks/useServer';
import { useUser } from '../hooks/useUser';
import type { CourseInfo } from '../types/CourseInfo';
import { useNavigate } from "react-router-dom";

export default function Sidebar() {

const {baseUrl} = useServer();
const {user} = useUser();
const [courses,setCourses] = useState<CourseInfo[]>([]);
const [open, setOpen] = useState(false);
const [role, setRole] = useState("Student");
const navigate = useNavigate();

useEffect(() => {
  if (!baseUrl || !user?.token) return;
  
  updateCourses();
  
}, [baseUrl, user,role]);

useEffect(() => updateRole(), []);

async function updateCourses(){
    let url:string = baseUrl + "/api/courses/mine";
  
  const res = await fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer "+user?.token
    }

  });

  if(!res.ok) throw Error("Could not fetch courses!");

  let data : CourseInfo[] = await res.json();

  setCourses(data);
}

function updateRole()
{
  if(user == null) return;
  
  if(user?.roles.toLowerCase().includes("student"))setRole("Student");
  if(user?.roles.toLowerCase().includes("teacher"))setRole("Teacher");
  if(user?.roles.toLowerCase().includes("admin"))setRole("Admin");
}

return (
  <>
    <IconButton
      onClick={() => setOpen(true)}
      sx={{ position: "fixed", top: 8, left: 8 }}
    >
      <MenuRoundedIcon />
    </IconButton>

    <Drawer
      variant="temporary"
      open={open}
      onClose={() => setOpen(false)}
    >
      <List>
  <ListItemButton
    onClick={() => {
      setOpen(false);
      navigate("/profile");
    }}
  >
    <ListItemText primary="Profile settings" />
  </ListItemButton>

  {role === "Admin" && <ListItemButton
    onClick={() => {
      setOpen(false);
       navigate("/users");
    }}
  >
    <ListItemText primary= {"Users"} />
  </ListItemButton>}

 <ListItemButton
    onClick={() => {
      setOpen(false);
       navigate((role !== "Student")? "/my-courses" : "/recent-submissions");
    }}
  >
    <ListItemText primary= {(role !== "Student")?"View All Courses":"Recent Submissions"} />
  </ListItemButton>

  {role==="Student" && <ListItemButton
    onClick={() => {
      setOpen(false);
       navigate("/grades");
    }}
  >
    <ListItemText primary= "My Grades" />
  </ListItemButton>}

 {role!=="Admin" && <ListItemButton disabled>
    <ListItemText primary="Courses:" />
  </ListItemButton>}

  {role!=="Admin" && courses.map((c) => (
    <ListItemButton
      key={c.id}
      onClick={() => {
        setOpen(false);
        navigate(`/courses/${c.id}`);
      }}
    >
      <Typography
        noWrap
        sx={{
          width: "100%",
          overflow: "hidden",
          textOverflow: "ellipsis",
        }}
      >
        {c.name}
      </Typography>
    </ListItemButton>
  ))}
</List>
    </Drawer>
  </>
);
}
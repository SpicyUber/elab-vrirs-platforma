import { useEffect, useState } from "react";
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Stack,
  CircularProgress,
  Button,
} from "@mui/material";

import type {UserProfileInfo} from "../types/UserProfileInfo";
import type {UserSearchResultPage} from "../types/UserSearchResultPage";

import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
import { useNavigate } from "react-router-dom";
import Sidebar from "../components/Sidebar";

const Users = () => {
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [index, setIndex] = useState("");

  const [pageNumber, setPageNumber] = useState(1);
  const [maxPages, setMaxPages] = useState(10);
  const [entriesPerPage] = useState(10);

  const [users, setUsers] = useState<UserProfileInfo[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const searchUsers = async () => {
    setLoading(true);
    setError(null);

    try {
      const params = new URLSearchParams();

      if (fullName) params.append("FullName", fullName);
      if (email) params.append("Email", email);
      if (index) params.append("Index", index);

      params.append("PageNumber", pageNumber.toString());
      params.append("EntriesPerPage", entriesPerPage.toString());

      const res = await fetch(
        `${baseUrl}/api/users/search?${params.toString()}`,
        {
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        }
      );

      if (!res.ok) {
        setError("Failed to fetch users");
        return;
      }

      const data = (await res.json()) as UserSearchResultPage;
      setUsers(data.userProfiles);
      setMaxPages(data.maxPages);
      setPageNumber(data.pageNumer);
    } catch {
      setError("Error while searching users");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    searchUsers();
  }, [pageNumber]);

  const deleteUser = async (userId: string) => {
    const res = await fetch(`${baseUrl}/api/users/${userId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${user?.token}`,
      },
    });

    if (!res.ok) {
      alert("Delete failed");
      return;
    }

    setUsers((prev) => prev.filter((u) => u.id !== userId));
  };

  return (
    <Box sx={{ padding: 3 }}>
        <Sidebar></Sidebar>
      <Typography variant="h4">Users</Typography>

      <Stack spacing={2} sx={{ mt: 2 }}>
        <TextField
          label="Full Name"
          value={fullName}
          onChange={(e) => setFullName(e.target.value)}
        />

        <TextField
          label="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <TextField
          label="Index"
          value={index}
          onChange={(e) => setIndex(e.target.value)}
        />

        <Button variant="contained" onClick={() => searchUsers()}>
          Search
        </Button>
      </Stack>

      {loading && <CircularProgress sx={{ mt: 2 }} />}

      {error && (
        <Typography color="error" sx={{ mt: 2 }}>
          {error}
        </Typography>
      )}

      <Stack spacing={2} sx={{ mt: 3 }}>
        {users.map((u) => (
          <Card key={u.id} variant="outlined">
            <CardContent>
              <Typography variant="h6">{u.fullName}</Typography>
              <Typography color="text.secondary">{u.email}</Typography>
              <Typography variant="caption">{u.indexNumber}</Typography>

              <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
                { (
                  <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
  <Button
    variant="contained"
    onClick={() => navigate(`/users/${u.id}/edit`)}
  >
    Edit
  </Button>

  {user?.roles?.includes("Admin") && (
    <Button
      color="error"
      variant="contained"
      onClick={() => deleteUser(u.id)}
    >
      Delete
    </Button>
  )}
</Stack>
                )}
              </Stack>
            </CardContent>
          </Card>
        ))}
      </Stack>

      <Stack direction="row" spacing={2} sx={{ mt: 3 }}>
        <Button
  disabled={pageNumber <= 1}
  onClick={() => setPageNumber(pageNumber - 1)}
>
  Prev
</Button>

<Typography>
  Page {pageNumber} / {maxPages}
</Typography>

<Button
  disabled={pageNumber >= maxPages}
  onClick={() => setPageNumber(pageNumber + 1)}
>
  Next
</Button>

      </Stack>
    </Box>
  );
};

export default Users;